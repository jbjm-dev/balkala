using BalkalaInfra.Application.DTOs.Request;
using BalkalaInfra.Application.DTOs.Response;

namespace BalkalaInfra.Application.Interfaces;

public interface IDashboardDefinitionService
{
    Task<IReadOnlyList<DashboardDefinitionResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<DashboardDefinitionResponse?> GetByIdAsync(ulong id, CancellationToken cancellationToken);
    Task<DashboardDefinitionResponse> CreateAsync(CreateDashboardDefinitionRequest request, CancellationToken cancellationToken);
    Task<DashboardDefinitionResponse?> UpdateAsync(ulong id, UpdateDashboardDefinitionRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(ulong id, int tenantId, CancellationToken cancellationToken);
}
