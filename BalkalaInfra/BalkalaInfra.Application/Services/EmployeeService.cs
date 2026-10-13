using BalkalaInfra.Application.DTOs.Request;
using BalkalaInfra.Application.DTOs.Response;
using BalkalaInfra.Application.Interfaces;
using BalkalaInfra.Domain.Entities;

namespace BalkalaInfra.Application.Services;

public sealed class EmployeeService(IEmployeeRepository repository) : IEmployeeService
{
    public async Task<IReadOnlyList<EmployeeListResponse>> GetAllAsync(CancellationToken ct) => (await repository.GetAllAsync(ct)).Select(MapList).ToArray();
    public async Task<IReadOnlyList<EmployeeListResponse>> GetActiveAsync(CancellationToken ct) => (await repository.GetActiveAsync(ct)).Select(MapList).ToArray();
    public async Task<EmployeeResponse?> GetByIdAsync(int id, CancellationToken ct) { var x = await repository.GetByIdAsync(id, ct); return x is null ? null : Map(x); }
    public async Task<EmployeeResponse> CreateAsync(CreateEmployeeRequest request, CancellationToken ct)
    {
        var x = Map(request); await repository.AddAsync(x, ct); return Map(x);
    }
    public async Task<EmployeeResponse?> UpdateAsync(int id, UpdateEmployeeRequest request, CancellationToken ct)
    {
        var x = await repository.GetByIdAsync(id, ct); if (x is null) return null;
        Apply(request, x); await repository.SaveChangesAsync(ct); return Map(x);
    }
    public async Task<EmployeeResponse?> UpdateStatusAsync(int id, string status, CancellationToken ct)
    {
        var x = await repository.GetByIdAsync(id, ct); if (x is null) return null;
        x.Status = status; await repository.SaveChangesAsync(ct); return Map(x);
    }
    public async Task<bool> DeactivateAsync(int id, CancellationToken ct)
    {
        var x = await repository.GetByIdAsync(id, ct); if (x is null) return false;
        x.Status = "Inactive"; await repository.SaveChangesAsync(ct); return true;
    }
    private static Employee Map(EmployeeRequestBase r) { var x = new Employee(); Apply(r, x); return x; }
    private static void Apply(EmployeeRequestBase r, Employee x) { x.TenantId=r.TenantId; x.OrganizationId=r.OrganizationId; x.EmployeeCode=r.EmployeeCode.Trim(); x.TenantUserId=r.TenantUserId; x.DepartmentId=r.DepartmentId; x.DesignationId=r.DesignationId; x.JoinDate=r.JoinDate; x.Status=string.IsNullOrWhiteSpace(r.Status)?"Active":r.Status; }
    private static EmployeeResponse Map(Employee x) => new(x.EmployeeId,x.TenantId,x.OrganizationId,x.EmployeeCode,x.TenantUserId,x.DepartmentId,x.DesignationId,x.JoinDate,x.Status);
    private static EmployeeListResponse MapList(Employee x) => new(x.EmployeeId,x.TenantId,x.OrganizationId,x.EmployeeCode,x.TenantUserId,x.DepartmentId,x.DesignationId,x.JoinDate,x.Status);
}
