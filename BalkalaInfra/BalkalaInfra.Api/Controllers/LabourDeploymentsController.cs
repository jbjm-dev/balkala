using BalkalaInfra.Application.DTOs.Request;
using BalkalaInfra.Application.DTOs.Response;
using BalkalaInfra.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BalkalaInfra.Api.Controllers;

[ApiController]
[Route("api/labourdeployments")]
public sealed class LabourDeploymentsController(ILabourDeploymentService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<LabourDeploymentResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<LabourDeploymentResponse>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await service.GetAllAsync(cancellationToken));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(LabourDeploymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LabourDeploymentResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return result is null
            ? NotFound(new ApiErrorResponse(false, "Not Found", $"Labour deployment with ID {id} was not found.", 40404))
            : Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(LabourDeploymentResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<LabourDeploymentResponse>> Create([FromBody] CreateLabourDeploymentRequest request, CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.LabourDeploymentId }, result);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(LabourDeploymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LabourDeploymentResponse>> Update(int id, [FromBody] UpdateLabourDeploymentRequest request, CancellationToken cancellationToken)
    {
        var result = await service.UpdateAsync(id, request, cancellationToken);
        return result is null
            ? NotFound(new ApiErrorResponse(false, "Not Found", $"Labour deployment with ID {id} was not found.", 40404))
            : Ok(result);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, [FromBody] DeleteLabourDeploymentRequest request, CancellationToken cancellationToken) =>
        await service.DeleteAsync(id, request.TenantId, cancellationToken)
            ? NoContent()
            : NotFound(new ApiErrorResponse(false, "Not Found", $"Labour deployment with ID {id} was not found.", 40404));
}
