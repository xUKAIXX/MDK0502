namespace RealEstateAgency.Domain;

public class Property
{
    public int Id { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string Address { get; private set; } = string.Empty;

    public decimal Price { get; private set; }

    public double Area { get; private set; }

    public int Rooms { get; private set; }

    public int Floor { get; private set; }

    public int TotalFloors { get; private set; }

    public int YearBuilt { get; private set; }

    public string? Description { get; private set; }

    public PropertyStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public int PropertyTypeId { get; private set; }

    public PropertyType PropertyType { get; private set; } = null!;

    public int DistrictId { get; private set; }

    public District District { get; private set; } = null!;

    public int AgentId { get; private set; }

    public Agent Agent { get; private set; } = null!;

    public string? OwnerId { get; private set; }

    public string? OwnerName { get; private set; }

    private Property()
    {
    }

    public static Property Create(
        string title,
        string address,
        decimal price,
        double area,
        int rooms,
        int floor,
        int totalFloors,
        int yearBuilt,
        string? description,
        PropertyType propertyType,
        District district,
        Agent agent,
        string? ownerId,
        string? ownerName,
        DateTime createdAt)
    {
        if (propertyType is null)
        {
            throw new DomainException("Тип объекта обязателен.");
        }

        if (district is null)
        {
            throw new DomainException("Район обязателен.");
        }

        if (agent is null)
        {
            throw new DomainException("Агент обязателен.");
        }

        Validate(title, address, price, area, rooms, floor, totalFloors, yearBuilt);

        return new Property
        {
            Title = title.Trim(),
            Address = address.Trim(),
            Price = price,
            Area = area,
            Rooms = rooms,
            Floor = floor,
            TotalFloors = totalFloors,
            YearBuilt = yearBuilt,
            Description = NormalizeDescription(description),
            Status = PropertyStatus.ForSale,
            CreatedAt = createdAt,
            PropertyTypeId = propertyType.Id,
            PropertyType = propertyType,
            DistrictId = district.Id,
            District = district,
            AgentId = agent.Id,
            Agent = agent,
            OwnerId = ownerId,
            OwnerName = string.IsNullOrWhiteSpace(ownerName) ? null : ownerName.Trim()
        };
    }

    public void UpdateDetails(
        string title,
        string address,
        decimal price,
        double area,
        int rooms,
        int floor,
        int totalFloors,
        int yearBuilt,
        string? description,
        PropertyType propertyType,
        District district,
        Agent agent)
    {
        EnsureEditable();

        if (propertyType is null)
        {
            throw new DomainException("Тип объекта обязателен.");
        }

        if (district is null)
        {
            throw new DomainException("Район обязателен.");
        }

        if (agent is null)
        {
            throw new DomainException("Агент обязателен.");
        }

        Validate(title, address, price, area, rooms, floor, totalFloors, yearBuilt);

        Title = title.Trim();
        Address = address.Trim();
        Price = price;
        Area = area;
        Rooms = rooms;
        Floor = floor;
        TotalFloors = totalFloors;
        YearBuilt = yearBuilt;
        Description = NormalizeDescription(description);
        PropertyTypeId = propertyType.Id;
        PropertyType = propertyType;
        DistrictId = district.Id;
        District = district;
        AgentId = agent.Id;
        Agent = agent;
    }

    public void ChangeStatus(PropertyStatus status)
    {
        if (Status is PropertyStatus.Sold or PropertyStatus.Rented)
        {
            throw new DomainException("Нельзя изменить статус проданного или сданного объекта.");
        }

        if (status == PropertyStatus.Sold && Status != PropertyStatus.ForSale)
        {
            throw new DomainException("Продать можно только объект, выставленный на продажу.");
        }

        if (status == PropertyStatus.Rented && Status != PropertyStatus.ForRent)
        {
            throw new DomainException("Сдать можно только объект, выставленный в аренду.");
        }

        Status = status;
    }

    public void MarkAsSold()
    {
        ChangeStatus(PropertyStatus.Sold);
    }

    public void MarkAsRented()
    {
        ChangeStatus(PropertyStatus.Rented);
    }

    private void EnsureEditable()
    {
        if (Status is PropertyStatus.Sold or PropertyStatus.Rented)
        {
            throw new DomainException("Нельзя редактировать проданный или сданный объект.");
        }
    }

    private static void Validate(
        string title,
        string address,
        decimal price,
        double area,
        int rooms,
        int floor,
        int totalFloors,
        int yearBuilt)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("Название объекта обязательно.");
        }

        if (title.Trim().Length is < 5 or > 150)
        {
            throw new DomainException("Название объекта должно быть от 5 до 150 символов.");
        }

        if (string.IsNullOrWhiteSpace(address))
        {
            throw new DomainException("Адрес объекта обязателен.");
        }

        if (address.Trim().Length is < 5 or > 200)
        {
            throw new DomainException("Адрес объекта должен быть от 5 до 200 символов.");
        }

        if (price <= 0)
        {
            throw new DomainException("Цена объекта должна быть положительной.");
        }

        if (price > 1_000_000_000m)
        {
            throw new DomainException("Цена объекта не может превышать 1 000 000 000.");
        }

        if (area <= 0)
        {
            throw new DomainException("Площадь объекта должна быть положительной.");
        }

        if (area > 100_000)
        {
            throw new DomainException("Площадь объекта не может превышать 100 000 м².");
        }

        if (rooms is < 0 or > 50)
        {
            throw new DomainException("Количество комнат должно быть от 0 до 50.");
        }

        if (floor < 0 || floor > 200)
        {
            throw new DomainException("Этаж должен быть от 0 до 200.");
        }

        if (totalFloors is < 1 or > 300)
        {
            throw new DomainException("Количество этажей должно быть от 1 до 300.");
        }

        if (floor > totalFloors)
        {
            throw new DomainException("Этаж не может быть больше общего количества этажей.");
        }

        if (yearBuilt is < 1500 or > 2100)
        {
            throw new DomainException("Год постройки должен быть в диапазоне 1500–2100.");
        }
    }

    private static string? NormalizeDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return null;
        }

        var value = description.Trim();

        return value.Length > 2000 ? value[..2000] : value;
    }
}
