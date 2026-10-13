using BalkalaInfra.Application.DTOs.Request;
using BalkalaInfra.Application.DTOs.Response;
using BalkalaInfra.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BalkalaInfra.Api.Controllers;

[ApiController]
[Route("api/employee")]
public sealed class EmployeeController(IEmployeeService service) : ControllerBase
{
    [HttpGet] public async Task<ActionResult<IReadOnlyList<EmployeeListResponse>>> GetAll(CancellationToken ct) => Ok(await service.GetAllAsync(ct));
    [HttpGet("active")] public async Task<ActionResult<IReadOnlyList<EmployeeListResponse>>> GetActive(CancellationToken ct) => Ok(await service.GetActiveAsync(ct));
    [HttpGet("{id:int}")] public async Task<ActionResult<EmployeeResponse>> GetById(int id, CancellationToken ct) => await service.GetByIdAsync(id, ct) is { } x ? Ok(x) : NotFound(new ApiErrorResponse(false,"Not Found",$"Employee with ID {id} was not found.",40404));
    [HttpPost] public async Task<ActionResult<EmployeeResponse>> Create(CreateEmployeeRequest request, CancellationToken ct) { var x=await service.CreateAsync(request,ct); return CreatedAtAction(nameof(GetById),new {id=x.EmployeeId},x); }
    [HttpPut("{id:int}")] public async Task<ActionResult<EmployeeResponse>> Update(int id, UpdateEmployeeRequest request, CancellationToken ct) => await service.UpdateAsync(id,request,ct) is { } x ? Ok(x) : NotFound(new ApiErrorResponse(false,"Not Found",$"Employee with ID {id} was not found.",40404));
    [HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id, CancellationToken ct) => await service.DeactivateAsync(id,ct) ? NoContent() : NotFound(new ApiErrorResponse(false,"Not Found",$"Employee with ID {id} was not found.",40404));
    [HttpPatch("{id:int}/status")] public async Task<ActionResult<EmployeeResponse>> UpdateStatus(int id, UpdateEmployeeStatusRequest request, CancellationToken ct) => await service.UpdateStatusAsync(id,request.EmploymentStatus,ct) is { } x ? Ok(x) : NotFound(new ApiErrorResponse(false,"Not Found",$"Employee with ID {id} was not found.",40404));
}
