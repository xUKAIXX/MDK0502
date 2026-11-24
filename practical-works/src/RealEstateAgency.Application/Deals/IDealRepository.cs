using RealEstateAgency.Domain;

namespace RealEstateAgency.Application.Deals;

public interface IDealRepository
{
    Task<IReadOnlyList<Deal>> GetAllAsync(CancellationToken cancellationToken);

    Task<Deal?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task AddAsync(Deal deal, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
