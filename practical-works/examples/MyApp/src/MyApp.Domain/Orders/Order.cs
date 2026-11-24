namespace MyApp.Domain.Orders;

public sealed class Order
{
    private readonly List<OrderItem> _items = new();

    private Order(Guid id, Guid customerId, DateTime createdAt)
    {
        Id = id;
        CustomerId = customerId;
        CreatedAt = createdAt;
    }

    public Guid Id { get; }

    public Guid CustomerId { get; }

    public DateTime CreatedAt { get; }

    public OrderStatus Status { get; private set; } = OrderStatus.Draft;

    public IReadOnlyCollection<OrderItem> Items => _items;

    public decimal Total => _items.Sum(i => i.Total);

    public static Order Create(Guid customerId, DateTime now)
    {
        if (customerId == Guid.Empty)
        {
            throw new DomainException("Не указан заказчик.");
        }

        return new Order(Guid.NewGuid(), customerId, now);
    }

    public void AddItem(string name, decimal price, int quantity, int maxItems)
    {
        if (Status != OrderStatus.Draft)
        {
            throw new DomainException("Изменять можно только черновик заказа.");
        }

        if (quantity <= 0)
        {
            throw new DomainException("Количество должно быть положительным.");
        }

        if (_items.Count >= maxItems)
        {
            throw new DomainException($"В заказе не может быть больше {maxItems} позиций.");
        }

        _items.Add(new OrderItem(name, price, quantity));
    }

    public void Submit()
    {
        if (_items.Count == 0)
        {
            throw new DomainException("Нельзя отправить пустой заказ.");
        }

        Status = OrderStatus.Submitted;
    }
}
