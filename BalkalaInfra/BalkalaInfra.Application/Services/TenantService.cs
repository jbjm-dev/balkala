using BalkalaInfra.Application.DTOs.Request;
using BalkalaInfra.Application.DTOs.Response;
using BalkalaInfra.Application.Exceptions;
using BalkalaInfra.Application.Interfaces;
using BalkalaInfra.Domain.Entities;

namespace BalkalaInfra.Application.Services;

public sealed class TenantService(ITenantRepository repository) : ITenantService
{
    public async Task<IReadOnlyList<TenantResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var tenants = await repository.GetAllAsync(cancellationToken);
        return tenants.Select(ToResponse).ToArray();
    }

    public async Task<TenantResponse?> GetByIdAsync(int tenantId, CancellationToken cancellationToken)
    {
        var tenant = await repository.GetByIdAsync(tenantId, cancellationToken);
        return tenant is null ? null : ToResponse(tenant);
    }

    public async Task<TenantResponse> CreateAsync(CreateTenantRequest request, CancellationToken cancellationToken)
    {
        var code = request.TenantCode.Trim();
        if (await repository.TenantCodeExistsAsync(code, cancellationToken))
            throw new IdentityApiException(409, "Conflict", $"Tenant code '{code}' already exists.", 40901);

        var status = string.IsNullOrWhiteSpace(request.Status) ? "Active" : request.Status.Trim();
        if (status is not ("Active" or "Inactive" or "Suspended"))
            throw new ArgumentException("Status must be Active, Inactive, or Suspended.");
        var deploymentModel = string.IsNullOrWhiteSpace(request.DeploymentModel) ? "Shared" : request.DeploymentModel.Trim();
        if (deploymentModel is not ("Shared" or "Dedicated"))
            throw new ArgumentException("Deployment model must be Shared or Dedicated.");

        var tenant = new Tenant
        {
            TenantCode = code,
            Name = request.Name.Trim(),
            Status = status,
            CreatedAt = DateTime.UtcNow,
            DeploymentModel = deploymentModel,
            DefaultTimeZone = request.DefaultTimeZone ?? "Asia/Kolkata",
            DefaultCurrencyId = request.DefaultCurrencyId
        };

        await repository.AddAsync(tenant, cancellationToken);
        return ToResponse(tenant);
    }

    private static TenantResponse ToResponse(Tenant tenant) =>
        new(tenant.TenantId, tenant.TenantCode, tenant.Name, tenant.Status, tenant.CreatedAt);
}
