using BalkalaInfra.Application.DTOs.Request;
using BalkalaInfra.Application.DTOs.Response;
using BalkalaInfra.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BalkalaInfra.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/clientinquiry")]
public sealed class ClientInquiryController(IClientInquiryService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ClientInquiryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ClientInquiryDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await service.GetAllAsync(cancellationToken));

    [HttpGet("{inquiryId:int}")]
    [ProducesResponseType(typeof(ClientInquiryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClientInquiryDto>> GetById(int inquiryId, CancellationToken cancellationToken)
    {
        var inquiry = await service.GetByIdAsync(inquiryId, cancellationToken);
        return inquiry is null
            ? NotFound(new ApiErrorResponse(false, "Not Found", $"Client inquiry with ID {inquiryId} was not found.", 40404))
            : Ok(inquiry);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ClientInquiryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClientInquiryDto>> Create(
        CreateClientInquiryRequest request, CancellationToken cancellationToken)
    {
        var inquiry = await service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { inquiryId = inquiry.InquiryId }, inquiry);
    }

    [HttpPut("{inquiryId:int}")]
    [ProducesResponseType(typeof(ClientInquiryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClientInquiryDto>> Update(
        int inquiryId, UpdateClientInquiryRequest request, CancellationToken cancellationToken)
    {
        var inquiry = await service.UpdateAsync(inquiryId, request, cancellationToken);
        return inquiry is null
            ? NotFound(new ApiErrorResponse(false, "Not Found", $"Client inquiry with ID {inquiryId} was not found.", 40404))
            : Ok(inquiry);
    }

    [HttpDelete("{inquiryId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int inquiryId, CancellationToken cancellationToken) =>
        await service.DeleteAsync(inquiryId, cancellationToken)
            ? NoContent()
            : NotFound(new ApiErrorResponse(false, "Not Found", $"Client inquiry with ID {inquiryId} was not found.", 40404));
}
