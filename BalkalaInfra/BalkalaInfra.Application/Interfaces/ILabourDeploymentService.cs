using BalkalaInfra.Application.DTOs.Request;
using BalkalaInfra.Application.DTOs.Response;

namespace BalkalaInfra.Application.Interfaces;

public interface ILabourDeploymentService
{
    Task<IReadOnlyList<LabourDeploymentResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<LabourDeploymentResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<LabourDeploymentResponse> CreateAsync(CreateLabourDeploymentRequest request, CancellationToken cancellationToken);
    Task<LabourDeploymentResponse?> UpdateAsync(int id, UpdateLabourDeploymentRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, int tenantId, CancellationToken cancellationToken);
}
