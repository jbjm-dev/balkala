using BalkalaInfra.Application.DTOs.Request;
using BalkalaInfra.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BalkalaInfra.Api.Controllers;

[ApiController, Route("api/users")]
public sealed class UsersController(IIdentityManagementService identity) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct) => Ok(await identity.GetUsers(ct));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct) => Ok(await identity.GetUserAccount(id, ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        var user = await identity.CreateUser(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = user.UserId }, user);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest request, CancellationToken ct) =>
        Ok(await identity.UpdateUser(id, request, ct));

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> Status(int id, [FromBody] UpdateUserStatusRequest request, CancellationToken ct)
    {
        await identity.UpdateUserStatus(id, request, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await identity.DeleteUser(id, ct);
        return NoContent();
    }

    [HttpGet("{id:int}/roles")]
    public async Task<IActionResult> Roles(int id, CancellationToken ct) => Ok(await identity.GetUserRoles(id, ct));

    [HttpPost("{id:int}/roles")]
    public async Task<IActionResult> AssignRole(int id, [FromBody] AssignRoleRequest request, CancellationToken ct)
    {
        await identity.AssignRole(id, request, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}/roles/{roleId:int}")]
    public async Task<IActionResult> RemoveRole(int id, int roleId, [FromBody] RemoveUserRoleRequest request, CancellationToken ct)
    {
        await identity.RemoveRole(id, roleId, request.TenantId, ct);
        return NoContent();
    }
}
