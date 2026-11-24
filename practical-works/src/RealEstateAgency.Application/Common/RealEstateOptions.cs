namespace RealEstateAgency.Application.Common;

public sealed class RealEstateOptions
{
    public const string SectionName = "RealEstateAgency";

    public int DefaultPageSize { get; set; } = 6;

    public int MaxPageSize { get; set; } = 50;

    public decimal MaxPropertyPrice { get; set; } = 1_000_000_000m;

    public string DefaultCurrency { get; set; } = "RUB";

    public string MortgageApiBaseUrl { get; set; } = "http://localhost:5100";

    public int MortgageApiTimeoutSeconds { get; set; } = 5;
}
