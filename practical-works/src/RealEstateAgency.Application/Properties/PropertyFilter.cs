namespace RealEstateAgency.Application.Properties;

public sealed record PropertyFilter
{
    public string? Search { get; set; }

    public string? Status { get; set; }

    public int? PropertyTypeId { get; set; }

    public int? DistrictId { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public string? OwnerId { get; set; }

    public string? SortBy { get; set; }

    public string? SortOrder { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 6;
}
