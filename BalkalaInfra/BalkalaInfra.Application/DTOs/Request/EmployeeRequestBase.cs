using System.ComponentModel.DataAnnotations;

namespace BalkalaInfra.Application.DTOs.Request;

public abstract class EmployeeRequestBase
{
    [Range(1, int.MaxValue)] public int TenantId { get; init; }
    [Range(1, int.MaxValue)] public int OrganizationId { get; init; }
    [Required, StringLength(50)] public string EmployeeCode { get; init; } = string.Empty;
    public int? TenantUserId { get; init; }
    [Range(1, int.MaxValue)] public int DepartmentId { get; init; }
    [Range(1, int.MaxValue)] public int DesignationId { get; init; }
    public DateOnly JoinDate { get; init; }
    [RegularExpression("^(Active|Inactive|OnLeave|Resigned|Terminated)$")] public string? Status { get; init; } = "Active";
}
