namespace BalkalaInfra.Domain.Entities;

public sealed class TenantUser
{
    public int TenantUserId { get; set; }
    public int TenantId { get; set; }
    public int UserId { get; set; }
    public int? EmployeeId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public int? DefaultOrganizationId { get; set; }
    public Tenant Tenant { get; set; } = null!;
    public User User { get; set; } = null!;
    public ICollection<TenantUserRole> TenantUserRoles { get; set; } = new List<TenantUserRole>();
}
