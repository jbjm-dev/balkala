namespace BalkalaInfra.Application.DTOs.Response;

public sealed record EmployeeResponse(int EmployeeId, int TenantId, int OrganizationId, string EmployeeCode, int? TenantUserId, int DepartmentId, int DesignationId, DateOnly JoinDate, string? Status);
