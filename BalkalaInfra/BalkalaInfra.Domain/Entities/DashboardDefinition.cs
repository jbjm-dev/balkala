namespace BalkalaInfra.Domain.Entities;

public sealed class DashboardDefinition
{
    public ulong DashboardDefinitionId { get; set; }
    public int TenantId { get; set; }
    public string DashboardCode { get; set; } = string.Empty;
    public string DashboardName { get; set; } = string.Empty;
    public string AudienceType { get; set; } = string.Empty;
    public string LayoutJson { get; set; } = "{}";
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Tenant Tenant { get; set; } = null!;
}
