using RealEstateAgency.Application.Common;
using RealEstateAgency.Application.Contracts;
using RealEstateAgency.Application.Properties;
using RealEstateAgency.Domain;

namespace RealEstateAgency.Application.Deals;

public sealed class DealService : IDealService
{
    private readonly IDealRepository _deals;
    private readonly IPropertyRepository _properties;
    private readonly IClock _clock;

    public DealService(IDealRepository deals, IPropertyRepository properties, IClock clock)
    {
        _deals = deals;
        _properties = properties;
        _clock = clock;
    }

    public async Task<IReadOnlyList<DealDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var deals = await _deals.GetAllAsync(cancellationToken);
        return deals.Select(Map).ToList();
    }

    public async Task<DealDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var deal = await _deals.GetByIdAsync(id, cancellationToken);
        return deal is null ? null : Map(deal);
    }

    public async Task<DealDto> CreateAsync(CreateDealRequest request, CancellationToken cancellationToken)
    {
        var property = await _properties.GetByIdAsync(request.PropertyId, cancellationToken)
            ?? throw new DomainException("Объект недвижимости не найден.");

        var deal = Deal.Create(
            property,
            request.ClientName,
            request.ClientPhone,
            (DealType)request.Type,
            request.Amount,
            _clock.UtcNow,
            request.Comment);

        await _deals.AddAsync(deal, cancellationToken);
        await _deals.SaveChangesAsync(cancellationToken);

        return Map(deal);
    }

    public async Task<bool> StartAsync(int id, CancellationToken cancellationToken)
    {
        var deal = await _deals.GetByIdAsync(id, cancellationToken);
        if (deal is null)
        {
            return false;
        }

        deal.Start();
        await _deals.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> CompleteAsync(int id, CancellationToken cancellationToken)
    {
        var deal = await _deals.GetByIdAsync(id, cancellationToken);
        if (deal is null)
        {
            return false;
        }

        deal.Complete();

        if (deal.Type == DealType.Purchase)
        {
            deal.Property.MarkAsSold();
        }
        else
        {
            deal.Property.MarkAsRented();
        }

        await _deals.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> CancelAsync(int id, CancellationToken cancellationToken)
    {
        var deal = await _deals.GetByIdAsync(id, cancellationToken);
        if (deal is null)
        {
            return false;
        }

        deal.Cancel();
        await _deals.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static DealDto Map(Deal deal) => new(
        deal.Id,
        deal.PropertyId,
        deal.Property.Title,
        deal.ClientName,
        deal.ClientPhone,
        deal.Type.ToRussian(),
        deal.Status.ToRussian(),
        deal.Amount,
        deal.Comment,
        deal.CreatedAt);
}
