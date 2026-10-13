using BalkalaInfra.Application.DTOs.Request;
using BalkalaInfra.Application.DTOs.Response;
using BalkalaInfra.Application.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BalkalaInfra.Api.Controllers;

[ApiController, Route("api/auth"), Authorize]
public sealed class AuthController(IIdentityManagementService identity) : ControllerBase
{
    [AllowAnonymous, HttpPost("register")]
    [ProducesResponseType(typeof(IdentityUserResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<IdentityUserResponse>> Register([FromBody] RegisterRequest request, CancellationToken ct) =>
        StatusCode(StatusCodes.Status201Created, await identity.Register(request, ct));

    [AllowAnonymous, HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct) => Ok(await identity.Login(request, ct));

    [AllowAnonymous, HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request, CancellationToken ct) => Ok(await identity.Refresh(request, ct));

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var userId = GetClaimInt(ClaimTypes.NameIdentifier);
        var authorization = Request.Headers.Authorization.ToString();
        var token = authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) ? authorization[7..].Trim() : string.Empty;
        await identity.Logout(userId, token, ct);
        return NoContent();
    }

    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken ct) =>
        Ok(await identity.Me(GetClaimInt("tenant_id"), GetClaimInt(ClaimTypes.NameIdentifier), ct));

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken ct)
    {
        await identity.ChangePassword(GetClaimInt(ClaimTypes.NameIdentifier), request.CurrentPassword, request.NewPassword, ct);
        return NoContent();
    }

    private int GetClaimInt(string claimType) => int.TryParse(User.FindFirstValue(claimType), out var value)
        ? value
        : throw new UnauthorizedAccessException("The access token is missing a required identity claim.");
}
