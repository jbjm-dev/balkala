using BalkalaInfra.Domain.Entities;

namespace BalkalaInfra.Application.Interfaces;

public interface IClientInquiryRepository
{
    Task<IReadOnlyList<ClientInquiry>> GetAllAsync(CancellationToken cancellationToken);
    Task<ClientInquiry?> GetByIdAsync(int inquiryId, CancellationToken cancellationToken);
    Task AddAsync(ClientInquiry inquiry, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
    void Delete(ClientInquiry inquiry);
}
