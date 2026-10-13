using BalkalaInfra.Application.DTOs.Request;
using BalkalaInfra.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BalkalaInfra.Api.Controllers;

[ApiController, Route("api/permissions")]
public sealed class PermissionsController(IIdentityManagementService identity) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Get(CancellationToken ct) => Ok(await identity.GetPermissions(ct));
    [HttpGet("{id:int}")] public async Task<IActionResult> GetById(int id, CancellationToken ct) => Ok(await identity.GetPermission(id, ct));
    [HttpPost] public async Task<IActionResult> Create(CreatePermissionRequest request, CancellationToken ct) { var permission = await identity.CreatePermission(request, ct); return CreatedAtAction(nameof(GetById), new { id = permission.PermissionId }, permission); }
    [HttpPut("{id:int}")] public async Task<IActionResult> Update(int id, UpdatePermissionRequest request, CancellationToken ct) => Ok(await identity.UpdatePermission(id, request, ct));
    [HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id, CancellationToken ct) { await identity.DeletePermission(id, ct); return NoContent(); }
}
