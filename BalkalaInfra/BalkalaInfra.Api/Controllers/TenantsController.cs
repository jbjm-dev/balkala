using BalkalaInfra.Application.DTOs.Request;
using BalkalaInfra.Application.DTOs.Response;
using BalkalaInfra.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BalkalaInfra.Api.Controllers;

[ApiController]
[Route("api/tenants")]
public sealed class TenantsController(ITenantService tenantService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TenantResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TenantResponse>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await tenantService.GetAllAsync(cancellationToken));

    [HttpGet("{tenantId:int}")]
    [ProducesResponseType(typeof(TenantResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TenantResponse>> GetById(int tenantId, CancellationToken cancellationToken)
    {
        var tenant = await tenantService.GetByIdAsync(tenantId, cancellationToken);
        return tenant is null
            ? NotFound(new ApiErrorResponse(false, "Not Found", $"Tenant '{tenantId}' was not found.", 40404))
            : Ok(tenant);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TenantResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TenantResponse>> Create(CreateTenantRequest request, CancellationToken cancellationToken)
    {
        var tenant = await tenantService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { tenantId = tenant.TenantId }, tenant);
    }
}
