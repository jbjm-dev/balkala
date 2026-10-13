namespace BalkalaInfra.Domain.Entities;

public sealed class User
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Mobile { get; set; }
    public string IdentityProvider { get; set; } = "Local";
    public string? IdentityProviderSubject { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public string GlobalStatus { get; set; } = "Active";
    public DateTime? LastLoginAt { get; set; }
    public ICollection<TenantUser> TenantUsers { get; set; } = new List<TenantUser>();
    public ICollection<UserSession> UserSessions { get; set; } = new List<UserSession>();
}
