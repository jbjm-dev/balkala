using BalkalaInfra.Application.Interfaces;
using BalkalaInfra.Domain.Entities;
using BalkalaInfra.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BalkalaInfra.Infrastructure.Repositories;

public sealed class DashboardDefinitionRepository(AppDbContext db) : IDashboardDefinitionRepository
{
    public async Task<IReadOnlyList<DashboardDefinition>> GetAllAsync(CancellationToken cancellationToken) =>
        await db.DashboardDefinitions.AsNoTracking()
            .OrderBy(x => x.DashboardName).ToListAsync(cancellationToken);

    public Task<DashboardDefinition?> GetByIdAsync(ulong id, CancellationToken cancellationToken) =>
        db.DashboardDefinitions.FirstOrDefaultAsync(x => x.DashboardDefinitionId == id, cancellationToken);

    public Task<bool> CodeExistsAsync(int tenantId, string code, ulong? excludingId, CancellationToken cancellationToken) =>
        db.DashboardDefinitions.AnyAsync(x => x.TenantId == tenantId && x.DashboardCode == code
            && (!excludingId.HasValue || x.DashboardDefinitionId != excludingId.Value), cancellationToken);

    public async Task AddAsync(DashboardDefinition definition, CancellationToken cancellationToken)
    {
        await db.DashboardDefinitions.AddAsync(definition, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) => db.SaveChangesAsync(cancellationToken);
    public void Delete(DashboardDefinition definition) => db.DashboardDefinitions.Remove(definition);
}
