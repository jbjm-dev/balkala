namespace BalkalaInfra.Application.DTOs.Response;

public sealed record TenantResponse(int TenantId, string TenantCode, string Name, string Status, DateTime? CreatedAt);
