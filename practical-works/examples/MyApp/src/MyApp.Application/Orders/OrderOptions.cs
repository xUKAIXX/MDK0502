namespace MyApp.Application.Orders;

public sealed class OrderOptions
{
    public const string SectionName = "Orders";

    public int MaxItemsPerOrder { get; set; } = 10;

    public string DefaultCurrency { get; set; } = "RUB";
}
