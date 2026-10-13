using BalkalaInfra.Application.DTOs.Request;
using BalkalaInfra.Application.DTOs.Response;

namespace BalkalaInfra.Application.Interfaces;

public interface IClientInquiryService
{
    Task<IReadOnlyList<ClientInquiryDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<ClientInquiryDto?> GetByIdAsync(int inquiryId, CancellationToken cancellationToken);
    Task<ClientInquiryDto> CreateAsync(CreateClientInquiryRequest request, CancellationToken cancellationToken);
    Task<ClientInquiryDto?> UpdateAsync(int inquiryId, UpdateClientInquiryRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int inquiryId, CancellationToken cancellationToken);
}
