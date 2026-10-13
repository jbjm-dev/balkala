using System.Text.Json;

namespace BalkalaInfra.Application.DTOs.Response;

public sealed record DashboardDefinitionResponse(
    ulong DashboardDefinitionId,
    int TenantId,
    string DashboardCode,
    string DashboardName,
    string AudienceType,
    JsonElement LayoutJson,
    bool IsDefault,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt);
