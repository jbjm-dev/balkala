namespace BalkalaInfra.Domain.Entities;

public sealed class Role
{
    public int RoleId { get; set; }
    public int TenantId { get; set; }
    public string RoleCode { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string RoleType { get; set; } = "Custom";
    public bool IsSystemRole { get; set; }
    public Tenant Tenant { get; set; } = null!;
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    public ICollection<TenantUserRole> TenantUserRoles { get; set; } = new List<TenantUserRole>();
}
