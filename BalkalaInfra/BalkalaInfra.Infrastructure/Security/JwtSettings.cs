using System.Text;
using Microsoft.Extensions.Configuration;

namespace BalkalaInfra.Infrastructure.Security;

public sealed class JwtSettings(string issuer, string audience, string signingKey, int accessTokenMinutes)
{
    public string Issuer { get; } = issuer;
    public string Audience { get; } = audience;
    public string SigningKey { get; } = signingKey;
    public int AccessTokenMinutes { get; } = accessTokenMinutes;

    public static JwtSettings FromConfiguration(IConfiguration configuration)
    {
        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];
        var signingKey = configuration["Jwt:SigningKey"];
        if (string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
            throw new InvalidOperationException("Configure Jwt:Issuer and Jwt:Audience before starting the API.");
        if (string.IsNullOrWhiteSpace(signingKey) || Encoding.UTF8.GetByteCount(signingKey) < 32)
            throw new InvalidOperationException("JWT signing key is missing or too short. Configure Jwt:SigningKey in Development User Secrets or Jwt__SigningKey in the deployment secret store (minimum 32 bytes).");

        var accessTokenMinutes = int.TryParse(configuration["Jwt:AccessTokenMinutes"], out var configuredMinutes)
            ? configuredMinutes
            : 30;
        if (accessTokenMinutes is < 1 or > 1440)
            throw new InvalidOperationException("Jwt:AccessTokenMinutes must be between 1 and 1440.");

        return new JwtSettings(issuer, audience, signingKey, accessTokenMinutes);
    }
}
