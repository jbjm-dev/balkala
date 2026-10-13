using BalkalaInfra.Domain.Entities;

namespace BalkalaInfra.Application.Interfaces;

public interface ITenantRepository
{
    Task<IReadOnlyList<Tenant>> GetAllAsync(CancellationToken cancellationToken);
    Task<Tenant?> GetByIdAsync(int tenantId, CancellationToken cancellationToken);
    Task<bool> TenantCodeExistsAsync(string tenantCode, CancellationToken cancellationToken);
    Task AddAsync(Tenant tenant, CancellationToken cancellationToken);
}
