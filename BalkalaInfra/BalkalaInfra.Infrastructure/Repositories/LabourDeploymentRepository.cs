using BalkalaInfra.Application.Interfaces;
using BalkalaInfra.Domain.Entities;
using BalkalaInfra.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BalkalaInfra.Infrastructure.Repositories;

public sealed class LabourDeploymentRepository(AppDbContext db) : ILabourDeploymentRepository
{
    public async Task<IReadOnlyList<LabourDeployment>> GetAllAsync(CancellationToken cancellationToken) =>
        await db.LabourDeployments.AsNoTracking()
            .OrderByDescending(x => x.WorkDate).ThenBy(x => x.LabourDeploymentId).ToListAsync(cancellationToken);

    public Task<LabourDeployment?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        db.LabourDeployments.FirstOrDefaultAsync(x => x.LabourDeploymentId == id, cancellationToken);

    public Task<bool> DuplicateExistsAsync(int projectId, int workerId, DateOnly workDate, int shiftId, int? excludingId, CancellationToken cancellationToken) =>
        db.LabourDeployments.AnyAsync(x => x.ProjectId == projectId && x.WorkerId == workerId
            && x.WorkDate == workDate && x.ShiftId == shiftId
            && (!excludingId.HasValue || x.LabourDeploymentId != excludingId.Value), cancellationToken);

    public async Task AddAsync(LabourDeployment deployment, CancellationToken cancellationToken)
    {
        await db.LabourDeployments.AddAsync(deployment, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) => db.SaveChangesAsync(cancellationToken);
    public void Delete(LabourDeployment deployment) => db.LabourDeployments.Remove(deployment);
}
