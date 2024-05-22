using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace ServerTests.Common;

internal class MockJwtTokenService
{
    public string Issuer { get; }
    public SecurityKey SecurityKey { get; }

    private readonly SigningCredentials _signingCredentials;
    private readonly JwtSecurityTokenHandler _tokenHandler = new();
    private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();
    private static readonly byte[] _key = new byte[64];

    public MockJwtTokenService()
    {
        Issuer = Guid.NewGuid().ToString();

        _rng.GetBytes(_key);
        SecurityKey = new SymmetricSecurityKey(_key) { KeyId = Guid.NewGuid().ToString() };
        _signingCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
    }

    public string GenerateJwtToken(string? audience, string playerId)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = Issuer,
            Audience = audience,
            Expires = DateTime.UtcNow.AddMinutes(20),
            SigningCredentials = _signingCredentials,
            Subject = new ClaimsIdentity(new Claim[] { new("Id", playerId) }),
        };
        var token = _tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
        return _tokenHandler.WriteToken(token);
    }
}