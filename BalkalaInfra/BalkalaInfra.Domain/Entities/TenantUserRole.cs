namespace BalkalaInfra.Domain.Entities;

public sealed class TenantUserRole
{
    public int TenantUserRoleId { get; set; }
    public int TenantId { get; set; }
    public int TenantUserId { get; set; }
    public int RoleId { get; set; }
    public int? OrganizationId { get; set; }
    public int? ProjectId { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public Tenant Tenant { get; set; } = null!;
    public TenantUser TenantUser { get; set; } = null!;
    public Role Role { get; set; } = null!;
}
