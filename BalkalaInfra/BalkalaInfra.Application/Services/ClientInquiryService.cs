using BalkalaInfra.Application.DTOs.Request;
using BalkalaInfra.Application.DTOs.Response;
using BalkalaInfra.Application.Interfaces;
using BalkalaInfra.Domain.Entities;

namespace BalkalaInfra.Application.Services;

public sealed class ClientInquiryService(IClientInquiryRepository repository) : IClientInquiryService
{
    public async Task<IReadOnlyList<ClientInquiryDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var inquiries = await repository.GetAllAsync(cancellationToken);
        return inquiries.Select(ToDto).ToArray();
    }

    public async Task<ClientInquiryDto?> GetByIdAsync(int inquiryId, CancellationToken cancellationToken)
    {
        var inquiry = await repository.GetByIdAsync(inquiryId, cancellationToken);
        return inquiry is null ? null : ToDto(inquiry);
    }

    public async Task<ClientInquiryDto> CreateAsync(
        CreateClientInquiryRequest request, CancellationToken cancellationToken)
    {
        var inquiry = new ClientInquiry
        {
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone,
            Description = request.Description,
            CreatedDate = request.CreatedDate,
            Status = request.Status,
            Location = request.Location,
            Source = request.Source,
            ProjectLocation = request.ProjectLocation
        };

        await repository.AddAsync(inquiry, cancellationToken);
        return ToDto(inquiry);
    }

    public async Task<ClientInquiryDto?> UpdateAsync(
        int inquiryId, UpdateClientInquiryRequest request, CancellationToken cancellationToken)
    {
        var inquiry = await repository.GetByIdAsync(inquiryId, cancellationToken);
        if (inquiry is null) return null;

        inquiry.Name = request.Name;
        inquiry.Email = request.Email;
        inquiry.Phone = request.Phone;
        inquiry.Description = request.Description;
        inquiry.CreatedDate = request.CreatedDate;
        inquiry.Status = request.Status;
        inquiry.Location = request.Location;
        inquiry.Source = request.Source;
        inquiry.ProjectLocation = request.ProjectLocation;

        await repository.SaveChangesAsync(cancellationToken);
        return ToDto(inquiry);
    }

    public async Task<bool> DeleteAsync(int inquiryId, CancellationToken cancellationToken)
    {
        var inquiry = await repository.GetByIdAsync(inquiryId, cancellationToken);
        if (inquiry is null) return false;

        repository.Delete(inquiry);
        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static ClientInquiryDto ToDto(ClientInquiry inquiry) => new(
        inquiry.InquiryId,
        inquiry.Name,
        inquiry.Email,
        inquiry.Phone,
        inquiry.Description,
        inquiry.CreatedDate,
        inquiry.Status,
        inquiry.Location,
        inquiry.Source,
        inquiry.ProjectLocation);
}
