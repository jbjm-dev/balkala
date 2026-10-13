using BalkalaInfra.Domain.Entities;

namespace BalkalaInfra.Application.Interfaces;

public interface IDashboardDefinitionRepository
{
    Task<IReadOnlyList<DashboardDefinition>> GetAllAsync(CancellationToken cancellationToken);
    Task<DashboardDefinition?> GetByIdAsync(ulong id, CancellationToken cancellationToken);
    Task<bool> CodeExistsAsync(int tenantId, string code, ulong? excludingId, CancellationToken cancellationToken);
    Task AddAsync(DashboardDefinition definition, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
    void Delete(DashboardDefinition definition);
}
