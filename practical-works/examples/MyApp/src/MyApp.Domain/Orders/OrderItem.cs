namespace MyApp.Domain.Orders;

public sealed class OrderItem
{
    public OrderItem(string name, decimal price, int quantity)
    {
        Name = name;
        Price = price;
        Quantity = quantity;
    }

    public string Name { get; }

    public decimal Price { get; }

    public int Quantity { get; }

    public decimal Total => Price * Quantity;
}
