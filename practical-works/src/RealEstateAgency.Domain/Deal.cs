namespace RealEstateAgency.Domain;

public class Deal
{
    public int Id { get; private set; }

    public int PropertyId { get; private set; }

    public Property Property { get; private set; } = null!;

    public string ClientName { get; private set; } = string.Empty;

    public string ClientPhone { get; private set; } = string.Empty;

    public DealType Type { get; private set; }

    public DealStatus Status { get; private set; }

    public decimal Amount { get; private set; }

    public string? Comment { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private Deal()
    {
    }

    public static Deal Create(
        Property property,
        string clientName,
        string clientPhone,
        DealType type,
        decimal amount,
        DateTime createdAt,
        string? comment = null)
    {
        if (property is null)
        {
            throw new DomainException("Объект недвижимости обязателен.");
        }

        if (string.IsNullOrWhiteSpace(clientName))
        {
            throw new DomainException("Имя клиента обязательно.");
        }

        if (clientName.Trim().Length is < 2 or > 100)
        {
            throw new DomainException("Имя клиента должно быть от 2 до 100 символов.");
        }

        if (string.IsNullOrWhiteSpace(clientPhone))
        {
            throw new DomainException("Телефон клиента обязателен.");
        }

        if (clientPhone.Trim().Length is < 5 or > 30)
        {
            throw new DomainException("Телефон клиента должен быть от 5 до 30 символов.");
        }

        if (amount <= 0)
        {
            throw new DomainException("Сумма сделки должна быть положительной.");
        }

        if (property.Status is PropertyStatus.Sold or PropertyStatus.Rented)
        {
            throw new DomainException("Объект уже продан или сдан.");
        }

        if (type == DealType.Purchase && property.Status != PropertyStatus.ForSale)
        {
            throw new DomainException("Для покупки объект должен быть выставлен на продажу.");
        }

        if (type == DealType.Rent && property.Status != PropertyStatus.ForRent)
        {
            throw new DomainException("Для аренды объект должен быть выставлен в аренду.");
        }

        return new Deal
        {
            PropertyId = property.Id,
            Property = property,
            ClientName = clientName.Trim(),
            ClientPhone = clientPhone.Trim(),
            Type = type,
            Status = DealStatus.New,
            Amount = amount,
            Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim(),
            CreatedAt = createdAt
        };
    }

    public void Start()
    {
        if (Status != DealStatus.New)
        {
            throw new DomainException("В работу можно взять только новую сделку.");
        }

        Status = DealStatus.InProgress;
    }

    public void Complete()
    {
        if (Status == DealStatus.Completed)
        {
            throw new DomainException("Сделка уже завершена.");
        }

        if (Status == DealStatus.Cancelled)
        {
            throw new DomainException("Отменённую сделку нельзя завершить.");
        }

        Status = DealStatus.Completed;
    }

    public void Cancel()
    {
        if (Status == DealStatus.Completed)
        {
            throw new DomainException("Завершённую сделку нельзя отменить.");
        }

        Status = DealStatus.Cancelled;
    }
}
