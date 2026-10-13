using System.ComponentModel.DataAnnotations;

namespace BalkalaInfra.Application.DTOs.Request;

public sealed record CreateLabourDeploymentRequest(
    [Range(1, int.MaxValue)] int TenantId,
    [Required] int ProjectId,
    int? TaskId,
    [Required] int WorkerId,
    [Required] DateOnly WorkDate,
    [Required] int ShiftId,
    decimal? HoursWorked,
    decimal? QuantityCompleted,
    decimal? Rate);

public sealed record UpdateLabourDeploymentRequest(
    [Range(1, int.MaxValue)] int TenantId,
    [Required] int ProjectId,
    int? TaskId,
    [Required] int WorkerId,
    [Required] DateOnly WorkDate,
    [Required] int ShiftId,
    decimal? HoursWorked,
    decimal? QuantityCompleted,
    decimal? Rate);

public sealed record DeleteLabourDeploymentRequest([Range(1, int.MaxValue)] int TenantId);
