namespace BalkalaInfra.Domain.Entities;

public sealed class RolePermission
{
    public int RolePermissionId { get; set; }
    public int TenantId { get; set; }
    public int RoleId { get; set; }
    public int PermissionId { get; set; }
    public string AccessScope { get; set; } = "Own";
    public Tenant Tenant { get; set; } = null!;
    public Role Role { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}
