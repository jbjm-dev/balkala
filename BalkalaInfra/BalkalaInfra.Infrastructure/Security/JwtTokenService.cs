using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BalkalaInfra.Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace BalkalaInfra.Infrastructure.Security;

public sealed record JwtTokenResult(string Token, DateTime ExpiresAt);

public sealed class JwtTokenService(JwtSettings settings)
{
    public JwtTokenResult CreateAccessToken(User user, TenantUser membership, IReadOnlyCollection<string> roles)
    {
        var now = DateTime.UtcNow;
        var expiresAt = now.AddMinutes(settings.AccessTokenMinutes);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Name, user.Email),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("tenant_id", membership.TenantId.ToString()),
            new("tenant_user_id", membership.TenantUserId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
        };

        foreach (var role in roles.Distinct(StringComparer.OrdinalIgnoreCase))
            claims.Add(new Claim(ClaimTypes.Role, role));
        if (roles.Contains("Admin", StringComparer.OrdinalIgnoreCase))
            claims.Add(new Claim("permission", "*"));

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SigningKey)),
            SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(settings.Issuer, settings.Audience, claims,
            notBefore: now, expires: expiresAt, signingCredentials: credentials);
        return new JwtTokenResult(new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
