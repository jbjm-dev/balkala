using System.ComponentModel.DataAnnotations;

namespace BalkalaInfra.Application.DTOs.Request;

public sealed record CreateTenantRequest(
    [Required, MinLength(2), MaxLength(100)] string TenantCode,
    [Required, MinLength(2), MaxLength(200)] string Name,
    [MaxLength(30)] string Status = "Active",
    [MaxLength(20)] string? DeploymentModel = "Shared",
    [MaxLength(100)] string? DefaultTimeZone = "Asia/Kolkata",
    int? DefaultCurrencyId = null);
