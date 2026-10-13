using BalkalaInfra.Application.DTOs.Request;
using BalkalaInfra.Application.DTOs.Response;
using BalkalaInfra.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BalkalaInfra.Api.Controllers;

[ApiController]
[Route("api/dashboarddefinitions")]
public sealed class DashboardDefinitionsController(IDashboardDefinitionService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<DashboardDefinitionResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<DashboardDefinitionResponse>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await service.GetAllAsync(cancellationToken));

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(DashboardDefinitionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DashboardDefinitionResponse>> GetById(ulong id, CancellationToken cancellationToken)
    {
        if (id == 0) return BadRequest(new ApiErrorResponse(false, "Bad Request", "The dashboard definition ID must be greater than zero.", 40012));
        var result = await service.GetByIdAsync(id, cancellationToken);
        return result is null
            ? NotFound(new ApiErrorResponse(false, "Not Found", $"Dashboard definition with ID {id} was not found.", 40404))
            : Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(DashboardDefinitionResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<DashboardDefinitionResponse>> Create([FromBody] CreateDashboardDefinitionRequest request, CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.DashboardDefinitionId }, result);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(DashboardDefinitionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DashboardDefinitionResponse>> Update(ulong id, [FromBody] UpdateDashboardDefinitionRequest request, CancellationToken cancellationToken)
    {
        if (id == 0) return BadRequest(new ApiErrorResponse(false, "Bad Request", "The dashboard definition ID must be greater than zero.", 40012));
        var result = await service.UpdateAsync(id, request, cancellationToken);
        return result is null
            ? NotFound(new ApiErrorResponse(false, "Not Found", $"Dashboard definition with ID {id} was not found.", 40404))
            : Ok(result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(ulong id, [FromBody] DeleteDashboardDefinitionRequest request, CancellationToken cancellationToken)
    {
        if (id == 0) return BadRequest(new ApiErrorResponse(false, "Bad Request", "The dashboard definition ID must be greater than zero.", 40012));
        return await service.DeleteAsync(id, request.TenantId, cancellationToken)
            ? NoContent()
            : NotFound(new ApiErrorResponse(false, "Not Found", $"Dashboard definition with ID {id} was not found.", 40404));
    }
}
