using System.IdentityModel.Tokens.Jwt;
using Application;
using Application.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Server;
using Server.Services;
using SharedLibrary.MonopolyMap;
using System.Security.Claims;
using System.Text;
using Application.Usecases.ReadyRoom;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Server.Configurations;
using Server.DataModels;
using Server.Hubs.Monopoly;
using Server.Hubs.ReadyRoom;
using Server.Presenters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMonopolyServer();
builder.Services.AddMonopolyApplication();
builder.Services.AddSignalR();

const string corsPolicy = "CorsPolicy";
builder.Services.AddCors(options => options.AddPolicy(corsPolicy,
        configurePolicy =>
        {
            configurePolicy.AllowAnyHeader()
                   .AllowAnyMethod()
                   .SetIsOriginAllowed(_ => true)
                   .AllowCredentials();
        }));

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(nameof(JwtSettings)));
builder.Services.AddOptions<JwtBearerOptions>(nameof(JwtSettings.Internal))
    .Configure<IOptions<JwtSettings>>((options, settings) =>
    {
        var internalSettings = settings.Value.Internal;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = internalSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = internalSettings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(internalSettings.SecretKey)),
            ValidateLifetime = true
        };
    });
builder.Services.AddAuthentication(nameof(JwtSettings.Internal))
    .AddJwtBearer(nameof(JwtSettings.Internal));
// 如果 Bind Options 時需要依賴注入
// 如果是develop
// if (builder.Environment.IsDevelopment())
// {
//     builder.Services.AddScoped<IPlatformService, DevelopmentPlatformService>();
// }
// else
// {
//     builder.Services.AddScoped<IPlatformService, PlatformService>();
// }
// builder.Services.AddSingleton<PlatformJwtEvents>();
// builder.Services
//     .AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
//     .Configure<PlatformJwtEvents>((opt, jwtEvents) =>
//     {
//         builder.Configuration.Bind(nameof(JwtBearerOptions), opt);
//         opt.Events = jwtEvents;
//     });
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme);


var app = builder.Build();

app.UseCors(corsPolicy);
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<MonopolyHub>("/monopoly");
app.MapHub<ReadyRoomHub>("/ready-room");

app.MapGet("/health", () => Results.Ok());

// 開始遊戲
// app.MapPost("/games", async (HttpContext context, CancellationToken cancellationToken) =>
// {
//     var hostId = context.User.FindFirst(ClaimTypes.Sid)!.Value;
//     var payload = (await context.Request.ReadFromJsonAsync<CreateGameBodyPayload>())!;
//     var createGameUsecase = app.Services.CreateScope().ServiceProvider.GetRequiredService<CreateGameUsecase>();
//     var presenter = new DefaultPresenter<CreateGameResponse>();
//     await createGameUsecase.ExecuteAsync(
//         new CreateGameRequest(hostId, payload.Players.Select(x => x.Id).ToArray()),
//         presenter, cancellationToken);
//
//     var frontendBaseUrl = app.Configuration["FrontendBaseUrl"]!;
//
//     var url = $@"{frontendBaseUrl}games/{presenter.Value.GameId}";
//
//     await context.Response.WriteAsync(url);
// }).RequireAuthorization();

app.MapPost("/dev/room", async (
    [FromBody] CreateGameBodyPayload? payload, 
    ClaimsPrincipal user, 
    CreateReadyRoomUsecase createReadyRoomUsecase, 
    PlayerJoinReadyRoomUsecase playerJoinReadyRoomUsecase, 
    CancellationToken cancellationToken) =>
{
    var playerId = user.FindFirst(ClaimTypes.NameIdentifier)!.Value;
    var presenter = new DefaultPresenter<CreateReadyRoomResponse>();
    await createReadyRoomUsecase.ExecuteAsync(new CreateReadyRoomRequest(playerId), presenter, cancellationToken);
    if (payload?.PlayerIds.Length != 0 && payload?.PlayerIds is not null)
    {
        foreach (var id in payload.PlayerIds)
        {
            await playerJoinReadyRoomUsecase.ExecuteAsync(
                new PlayerJoinReadyRoomRequest(presenter.Value.RoomId, id),
                new NullPresenter<PlayerJoinReadyRoomResponse>(),
                cancellationToken);
        }    
    }
    
    var frontendBaseUrl = app.Configuration["FrontendBaseUrl"]!;
    var url = $"{frontendBaseUrl}ready-room/{presenter.Value.RoomId}";
    
    return Results.Ok(url);
}).RequireAuthorization();

app.MapPost("/dev/user", (string userName, IOptions<JwtSettings> jwtSettings) =>
{
    // generate user id
    var userId = Guid.NewGuid().ToString();
    
    // generate jwt token
    var tokenHandler = new JwtSecurityTokenHandler();
    var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Value.Internal.SecretKey));
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, userName)
        }),
        Expires = DateTime.UtcNow.AddMinutes(jwtSettings.Value.Internal.ExpiresInMinutes),
        Issuer = jwtSettings.Value.Internal.Issuer,
        Audience = jwtSettings.Value.Internal.Audience,
        SigningCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature)
    };
    var token = tokenHandler.CreateToken(tokenDescriptor);
    var tokenString = tokenHandler.WriteToken(token);
    return Results.Ok(tokenString);
});

app.MapGet("/map", (string mapId) =>
{
    var projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
    var jsonFilePath = Path.Combine(projectDirectory, "Maps", $"{mapId}.json");

    if (!File.Exists(jsonFilePath))
    {
        return Results.NotFound();
    }

    // read json file
    var json = File.ReadAllText(jsonFilePath);
    var data = MonopolyMap.Parse(json);
    return Results.Json(data, MonopolyMap.JsonSerializerOptions);
});

app.MapGet("/rooms", () =>
{
    var repository = app.Services.CreateScope().ServiceProvider.GetRequiredService<IQueryRepository>();
    return Results.Json(repository.GetRooms());
});

#if DEBUG
app.MapGet("/users", () =>
{
    var platformService = app.Services.CreateScope().ServiceProvider.GetRequiredService<IPlatformService>() as DevelopmentPlatformService;
    var users = platformService?.GetUsers().Select(user => new { user.Id, user.Token });
    return Results.Json(users);
});
#endif

app.Run();