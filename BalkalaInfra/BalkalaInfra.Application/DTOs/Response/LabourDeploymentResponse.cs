namespace BalkalaInfra.Application.DTOs.Response;

public sealed record LabourDeploymentResponse(
    int LabourDeploymentId,
    int TenantId,
    int ProjectId,
    int? TaskId,
    int WorkerId,
    DateOnly WorkDate,
    int ShiftId,
    decimal? HoursWorked,
    decimal? QuantityCompleted,
    decimal? Rate);
