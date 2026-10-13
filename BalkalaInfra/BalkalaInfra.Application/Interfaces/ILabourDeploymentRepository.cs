using BalkalaInfra.Domain.Entities;

namespace BalkalaInfra.Application.Interfaces;

public interface ILabourDeploymentRepository
{
    Task<IReadOnlyList<LabourDeployment>> GetAllAsync(CancellationToken cancellationToken);
    Task<LabourDeployment?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<bool> DuplicateExistsAsync(int projectId, int workerId, DateOnly workDate, int shiftId, int? excludingId, CancellationToken cancellationToken);
    Task AddAsync(LabourDeployment deployment, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
    void Delete(LabourDeployment deployment);
}
