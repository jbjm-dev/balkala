namespace BalkalaInfra.Domain.Entities;

public sealed class Tenant
{
    public int TenantId { get; set; }
    public string TenantCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public string DeploymentModel { get; set; } = "Shared";
    public string? DefaultTimeZone { get; set; } = "Asia/Kolkata";
    public int? DefaultCurrencyId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public ICollection<TenantUser> TenantUsers { get; set; } = new List<TenantUser>();
    public ICollection<Role> Roles { get; set; } = new List<Role>();
    public ICollection<UserSession> UserSessions { get; set; } = new List<UserSession>();
}
