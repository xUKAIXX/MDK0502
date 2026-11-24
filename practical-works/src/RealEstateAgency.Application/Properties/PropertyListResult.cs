using RealEstateAgency.Application.Contracts;

namespace RealEstateAgency.Application.Properties;

public sealed record PropertyListResult(
    IReadOnlyList<PropertyListItemDto> Items,
    int Total,
    int Page,
    int PageSize,
    int TotalPages);
