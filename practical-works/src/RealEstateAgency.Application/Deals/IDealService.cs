using RealEstateAgency.Application.Contracts;

namespace RealEstateAgency.Application.Deals;

public interface IDealService
{
    Task<IReadOnlyList<DealDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<DealDto?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<DealDto> CreateAsync(CreateDealRequest request, CancellationToken cancellationToken);

    Task<bool> StartAsync(int id, CancellationToken cancellationToken);

    Task<bool> CompleteAsync(int id, CancellationToken cancellationToken);

    Task<bool> CancelAsync(int id, CancellationToken cancellationToken);
}
