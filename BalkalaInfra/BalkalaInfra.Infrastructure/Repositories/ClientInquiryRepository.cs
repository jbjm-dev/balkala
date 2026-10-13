using BalkalaInfra.Application.Interfaces;
using BalkalaInfra.Domain.Entities;
using BalkalaInfra.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BalkalaInfra.Infrastructure.Repositories;

public sealed class ClientInquiryRepository(AppDbContext dbContext) : IClientInquiryRepository
{
    public async Task<IReadOnlyList<ClientInquiry>> GetAllAsync(CancellationToken cancellationToken) =>
        await dbContext.ClientInquiries
            .AsNoTracking()
            .OrderBy(inquiry => inquiry.InquiryId)
            .ToListAsync(cancellationToken);

    public Task<ClientInquiry?> GetByIdAsync(int inquiryId, CancellationToken cancellationToken) =>
        dbContext.ClientInquiries.FirstOrDefaultAsync(inquiry => inquiry.InquiryId == inquiryId, cancellationToken);

    public async Task AddAsync(ClientInquiry inquiry, CancellationToken cancellationToken)
    {
        await dbContext.ClientInquiries.AddAsync(inquiry, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);

    public void Delete(ClientInquiry inquiry) => dbContext.ClientInquiries.Remove(inquiry);
}
