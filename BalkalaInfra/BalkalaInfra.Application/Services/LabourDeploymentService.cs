using BalkalaInfra.Application.DTOs.Request;
using BalkalaInfra.Application.DTOs.Response;
using BalkalaInfra.Application.Exceptions;
using BalkalaInfra.Application.Interfaces;
using BalkalaInfra.Domain.Entities;

namespace BalkalaInfra.Application.Services;

public sealed class LabourDeploymentService(ILabourDeploymentRepository repository) : ILabourDeploymentService
{
    public async Task<IReadOnlyList<LabourDeploymentResponse>> GetAllAsync(CancellationToken cancellationToken) =>
        (await repository.GetAllAsync(cancellationToken)).Select(ToResponse).ToArray();

    public async Task<LabourDeploymentResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : ToResponse(entity);
    }

    public async Task<LabourDeploymentResponse> CreateAsync(CreateLabourDeploymentRequest request, CancellationToken cancellationToken)
    {
        await EnsureUnique(request.ProjectId, request.WorkerId, request.WorkDate, request.ShiftId, null, cancellationToken);
        var entity = new LabourDeployment
        {
            TenantId = request.TenantId,
            ProjectId = request.ProjectId,
            TaskId = request.TaskId,
            WorkerId = request.WorkerId,
            WorkDate = request.WorkDate,
            ShiftId = request.ShiftId,
            HoursWorked = request.HoursWorked,
            QuantityCompleted = request.QuantityCompleted,
            Rate = request.Rate
        };
        await repository.AddAsync(entity, cancellationToken);
        return ToResponse(entity);
    }

    public async Task<LabourDeploymentResponse?> UpdateAsync(int id, UpdateLabourDeploymentRequest request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.TenantId != request.TenantId) return null;
        await EnsureUnique(request.ProjectId, request.WorkerId, request.WorkDate, request.ShiftId, id, cancellationToken);
        entity.ProjectId = request.ProjectId;
        entity.TaskId = request.TaskId;
        entity.WorkerId = request.WorkerId;
        entity.WorkDate = request.WorkDate;
        entity.ShiftId = request.ShiftId;
        entity.HoursWorked = request.HoursWorked;
        entity.QuantityCompleted = request.QuantityCompleted;
        entity.Rate = request.Rate;
        await repository.SaveChangesAsync(cancellationToken);
        return ToResponse(entity);
    }

    public async Task<bool> DeleteAsync(int id, int tenantId, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.TenantId != tenantId) return false;
        repository.Delete(entity);
        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task EnsureUnique(int projectId, int workerId, DateOnly workDate, int shiftId, int? excludingId, CancellationToken cancellationToken)
    {
        if (await repository.DuplicateExistsAsync(projectId, workerId, workDate, shiftId, excludingId, cancellationToken))
            throw new IdentityApiException(409, "Conflict", "A labour deployment already exists for this project, worker, work date, and shift.", 40901);
    }

    private static LabourDeploymentResponse ToResponse(LabourDeployment entity) => new(
        entity.LabourDeploymentId, entity.TenantId, entity.ProjectId, entity.TaskId, entity.WorkerId,
        entity.WorkDate, entity.ShiftId, entity.HoursWorked, entity.QuantityCompleted, entity.Rate);
}
