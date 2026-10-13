using System.ComponentModel.DataAnnotations;

namespace BalkalaInfra.Application.DTOs.Request;

public sealed class UpdateEmployeeStatusRequest
{
    [Required, RegularExpression("^(Active|Inactive|On Leave)$")]
    public string EmploymentStatus { get; init; } = string.Empty;
}
