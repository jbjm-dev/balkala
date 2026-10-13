using System.Text.Json;
using BalkalaInfra.Application.DTOs.Request;
using BalkalaInfra.Application.DTOs.Response;
using BalkalaInfra.Application.Exceptions;
using BalkalaInfra.Application.Interfaces;
using BalkalaInfra.Domain.Entities;

namespace BalkalaInfra.Application.Services;

public sealed class DashboardDefinitionService(IDashboardDefinitionRepository repository) : IDashboardDefinitionService
{
    public async Task<IReadOnlyList<DashboardDefinitionResponse>> GetAllAsync(CancellationToken cancellationToken) =>
        (await repository.GetAllAsync(cancellationToken)).Select(ToResponse).ToArray();

    public async Task<DashboardDefinitionResponse?> GetByIdAsync(ulong id, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : ToResponse(entity);
    }

    public async Task<DashboardDefinitionResponse> CreateAsync(CreateDashboardDefinitionRequest request, CancellationToken cancellationToken)
    {
        ValidateLayout(request.LayoutJson);
        var code = request.DashboardCode.Trim();
        if (await repository.CodeExistsAsync(request.TenantId, code, null, cancellationToken))
            throw Conflict("A dashboard with this code already exists for the tenant.");

        var now = DateTime.UtcNow;
        var entity = new DashboardDefinition
        {
            TenantId = request.TenantId,
            DashboardCode = code,
            DashboardName = request.DashboardName.Trim(),
            AudienceType = request.AudienceType.Trim(),
            LayoutJson = request.LayoutJson.GetRawText(),
            IsDefault = request.IsDefault,
            IsActive = request.IsActive,
            CreatedAt = now,
            UpdatedAt = now
        };
        await repository.AddAsync(entity, cancellationToken);
        return ToResponse(entity);
    }

    public async Task<DashboardDefinitionResponse?> UpdateAsync(ulong id, UpdateDashboardDefinitionRequest request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.TenantId != request.TenantId) return null;
        ValidateLayout(request.LayoutJson);
        var code = request.DashboardCode.Trim();
        if (await repository.CodeExistsAsync(request.TenantId, code, id, cancellationToken))
            throw Conflict("A dashboard with this code already exists for the tenant.");

        entity.DashboardCode = code;
        entity.DashboardName = request.DashboardName.Trim();
        entity.AudienceType = request.AudienceType.Trim();
        entity.LayoutJson = request.LayoutJson.GetRawText();
        entity.IsDefault = request.IsDefault;
        entity.IsActive = request.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);
        return ToResponse(entity);
    }

    public async Task<bool> DeleteAsync(ulong id, int tenantId, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.TenantId != tenantId) return false;
        repository.Delete(entity);
        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static void ValidateLayout(JsonElement layout)
    {
        if (layout.ValueKind == JsonValueKind.Undefined)
            throw new IdentityApiException(400, "Bad Request", "The 'layoutJson' field is required.", 40012);
    }

    private static DashboardDefinitionResponse ToResponse(DashboardDefinition entity) => new(
        entity.DashboardDefinitionId, entity.TenantId, entity.DashboardCode, entity.DashboardName,
        entity.AudienceType, JsonDocument.Parse(entity.LayoutJson).RootElement.Clone(), entity.IsDefault,
        entity.IsActive, entity.CreatedAt, entity.UpdatedAt);

    private static IdentityApiException Conflict(string message) => new(409, "Conflict", message, 40901);
}
