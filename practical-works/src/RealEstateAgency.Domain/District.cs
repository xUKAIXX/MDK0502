namespace RealEstateAgency.Domain;

public class District
{
    private readonly List<Property> _properties = new();

    public int Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string City { get; private set; } = string.Empty;

    public IReadOnlyCollection<Property> Properties => _properties;

    private District()
    {
    }

    public static District Create(string name, string city)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Название района обязательно.");
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            throw new DomainException("Город обязателен.");
        }

        return new District
        {
            Name = name.Trim(),
            City = city.Trim()
        };
    }

    public void Rename(string name, string city)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Название района обязательно.");
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            throw new DomainException("Город обязателен.");
        }

        Name = name.Trim();
        City = city.Trim();
    }
}
