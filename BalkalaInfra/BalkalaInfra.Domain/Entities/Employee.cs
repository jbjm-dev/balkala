namespace BalkalaInfra.Domain.Entities;

public sealed class Employee
{
    public int EmployeeId { get; set; }
    public int TenantId { get; set; }
    public int OrganizationId { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public int? TenantUserId { get; set; }
    public int DepartmentId { get; set; }
    public int DesignationId { get; set; }
    public DateOnly JoinDate { get; set; }
    public string? Status { get; set; } = "Active";
}
