using RealEstateAgency.Application.Contracts;

namespace RealEstateAgency.Web.ViewModels;

public sealed class HomeViewModel
{
    public int TotalProperties { get; set; }

    public int TotalDeals { get; set; }

    public IReadOnlyList<PropertyListItemDto> RecentProperties { get; set; } = Array.Empty<PropertyListItemDto>();
}
