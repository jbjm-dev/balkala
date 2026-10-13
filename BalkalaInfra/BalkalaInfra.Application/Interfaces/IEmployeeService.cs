using BalkalaInfra.Application.DTOs.Request;
using BalkalaInfra.Application.DTOs.Response;
namespace BalkalaInfra.Application.Interfaces;
public interface IEmployeeService
{
    Task<IReadOnlyList<EmployeeListResponse>> GetAllAsync(CancellationToken ct);
    Task<IReadOnlyList<EmployeeListResponse>> GetActiveAsync(CancellationToken ct);
    Task<EmployeeResponse?> GetByIdAsync(int id, CancellationToken ct);
    Task<EmployeeResponse> CreateAsync(CreateEmployeeRequest request, CancellationToken ct);
    Task<EmployeeResponse?> UpdateAsync(int id, UpdateEmployeeRequest request, CancellationToken ct);
    Task<EmployeeResponse?> UpdateStatusAsync(int id, string status, CancellationToken ct);
    Task<bool> DeactivateAsync(int id, CancellationToken ct);
}
