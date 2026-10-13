namespace BalkalaInfra.Domain.Entities;

public sealed class UserSession
{
    public int UserSessionId { get; set; }
    public int UserId { get; set; }
    public int? TenantId { get; set; }
    public string SessionTokenHash { get; set; } = string.Empty;
    public string RefreshTokenHash { get; set; } = string.Empty;
    public string? DeviceId { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime RefreshExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public User User { get; set; } = null!;
    public Tenant? Tenant { get; set; }
}
