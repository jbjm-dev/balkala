using BalkalaInfra.Application.DTOs.Request;
using BalkalaInfra.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BalkalaInfra.Api.Controllers;

[ApiController, Route("api/roles")]
public sealed class RolesController(IIdentityManagementService identity) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct) => Ok(await identity.GetRoles(ct));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct) => Ok(await identity.GetRole(id, ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequest request, CancellationToken ct)
    {
        var role = await identity.CreateRole(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = role.RoleId }, role);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRoleRequest request, CancellationToken ct) =>
        Ok(await identity.UpdateRole(id, request, ct));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, [FromBody] DeleteRoleRequest request, CancellationToken ct)
    {
        await identity.DeleteRole(id, request.TenantId, ct);
        return NoContent();
    }

    [HttpGet("{id:int}/permissions")]
    public async Task<IActionResult> Permissions(int id, CancellationToken ct) => Ok(await identity.GetRolePermissions(id, ct));

    [HttpPut("{id:int}/permissions")]
    public async Task<IActionResult> SetPermissions(int id, [FromBody] UpdateRolePermissionsRequest request, CancellationToken ct)
    {
        await identity.SetRolePermissions(id, request, ct);
        return NoContent();
    }
}
