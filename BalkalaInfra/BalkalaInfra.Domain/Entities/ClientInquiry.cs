namespace BalkalaInfra.Domain.Entities;

public sealed class ClientInquiry
{
    public int InquiryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public string Source { get; set; } = string.Empty;

    public string ProjectLocation { get; set; } = string.Empty;
}
