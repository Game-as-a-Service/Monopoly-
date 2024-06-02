namespace Monopoly.InterfaceAdapterLayer.Server.Configurations;

public class JwtSettings
{
    public required JwtConfigurations Internal { get; set; }

    public class JwtConfigurations
    {
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public required string SecretKey { get; set; }
        public required int ExpiresInMinutes { get; set; }
    }
}