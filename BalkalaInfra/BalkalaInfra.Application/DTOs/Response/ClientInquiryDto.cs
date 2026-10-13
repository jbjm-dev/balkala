namespace BalkalaInfra.Application.DTOs.Response;

public sealed record ClientInquiryDto(
    int InquiryId,
    string Name,
    string Email,
    string Phone,
    string Description,
    DateTime CreatedDate,
    string Status,
    string Location,
    string Source,
    string ProjectLocation);

