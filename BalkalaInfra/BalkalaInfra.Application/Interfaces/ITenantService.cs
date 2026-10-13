using BalkalaInfra.Application.DTOs.Request;
using BalkalaInfra.Application.DTOs.Response;

namespace BalkalaInfra.Application.Interfaces;

public interface ITenantService
{
    Task<IReadOnlyList<TenantResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<TenantResponse?> GetByIdAsync(int tenantId, CancellationToken cancellationToken);
    Task<TenantResponse> CreateAsync(CreateTenantRequest request, CancellationToken cancellationToken);
}
