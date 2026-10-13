using BalkalaInfra.Domain.Entities;
namespace BalkalaInfra.Application.Interfaces;
public interface IEmployeeRepository
{
    Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken ct);
    Task<IReadOnlyList<Employee>> GetActiveAsync(CancellationToken ct);
    Task<Employee?> GetByIdAsync(int id, CancellationToken ct);
    Task AddAsync(Employee employee, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
