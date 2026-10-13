using BalkalaInfra.Application.Interfaces;
using BalkalaInfra.Domain.Entities;
using BalkalaInfra.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace BalkalaInfra.Infrastructure.Repositories;
public sealed class EmployeeRepository(AppDbContext db) : IEmployeeRepository
{
    public async Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken ct) => await db.Employees.AsNoTracking().OrderBy(x => x.EmployeeId).ToListAsync(ct);
    public async Task<IReadOnlyList<Employee>> GetActiveAsync(CancellationToken ct) => await db.Employees.AsNoTracking().Where(x => x.Status == "Active").OrderBy(x => x.EmployeeId).ToListAsync(ct);
    public Task<Employee?> GetByIdAsync(int id, CancellationToken ct) => db.Employees.FirstOrDefaultAsync(x => x.EmployeeId == id, ct);
    public async Task AddAsync(Employee employee, CancellationToken ct) { await db.Employees.AddAsync(employee, ct); await db.SaveChangesAsync(ct); }
    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}
