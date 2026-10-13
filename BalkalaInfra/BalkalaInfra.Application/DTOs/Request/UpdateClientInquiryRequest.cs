using System.ComponentModel.DataAnnotations;

namespace BalkalaInfra.Application.DTOs.Request;

public sealed record UpdateClientInquiryRequest(
    [Required] string Name,
    [Required, EmailAddress] string Email,
    [Required] string Phone,
    [Required] string Description,
    DateTime CreatedDate,
    [Required] string Status,
    [Required] string Location,
    [Required] string Source,
    [Required, StringLength(200)] string ProjectLocation);

