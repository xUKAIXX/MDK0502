using Microsoft.EntityFrameworkCore;
using RealEstateAgency.Application.Deals;
using RealEstateAgency.Domain;

namespace RealEstateAgency.Infrastructure.Persistence.Repositories;

public sealed class DealRepository(ApplicationDbContext context) : IDealRepository
{
    public async Task<IReadOnlyList<Deal>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.Deals
            .AsNoTracking()
            .Include(deal => deal.Property)
            .OrderByDescending(deal => deal.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Deal?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await context.Deals
            .Include(deal => deal.Property)
            .FirstOrDefaultAsync(deal => deal.Id == id, cancellationToken);
    }

    public async Task AddAsync(Deal deal, CancellationToken cancellationToken)
    {
        await context.Deals.AddAsync(deal, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}
