using BalkalaInfra.Application.Interfaces;
using BalkalaInfra.Domain.Entities;
using BalkalaInfra.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BalkalaInfra.Infrastructure.Repositories;

public sealed class TenantRepository(AppDbContext dbContext) : ITenantRepository
{
    public async Task<IReadOnlyList<Tenant>> GetAllAsync(CancellationToken cancellationToken) =>
        await dbContext.Tenants.AsNoTracking().OrderBy(x => x.Name).ToListAsync(cancellationToken);

    public Task<Tenant?> GetByIdAsync(int tenantId, CancellationToken cancellationToken) =>
        dbContext.Tenants.AsNoTracking().FirstOrDefaultAsync(x => x.TenantId == tenantId, cancellationToken);

    public Task<bool> TenantCodeExistsAsync(string tenantCode, CancellationToken cancellationToken) =>
        dbContext.Tenants.AnyAsync(x => x.TenantCode == tenantCode, cancellationToken);

    public async Task AddAsync(Tenant tenant, CancellationToken cancellationToken)
    {
        await dbContext.Tenants.AddAsync(tenant, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
