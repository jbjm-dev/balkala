using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace BalkalaInfra.Application.DTOs.Request;

public sealed record CreateDashboardDefinitionRequest(
    [Range(1, int.MaxValue)] int TenantId,
    [Required, MaxLength(100)] string DashboardCode,
    [Required, MaxLength(255)] string DashboardName,
    [Required, MaxLength(50)] string AudienceType,
    [Required] JsonElement LayoutJson,
    bool IsDefault,
    bool IsActive);

public sealed record UpdateDashboardDefinitionRequest(
    [Range(1, int.MaxValue)] int TenantId,
    [Required, MaxLength(100)] string DashboardCode,
    [Required, MaxLength(255)] string DashboardName,
    [Required, MaxLength(50)] string AudienceType,
    [Required] JsonElement LayoutJson,
    bool IsDefault,
    bool IsActive);

public sealed record DeleteDashboardDefinitionRequest(
    [Range(1, int.MaxValue)] int TenantId);
