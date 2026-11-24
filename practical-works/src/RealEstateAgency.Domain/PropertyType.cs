namespace RealEstateAgency.Domain;

public class PropertyType
{
    private readonly List<Property> _properties = new();

    public int Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public IReadOnlyCollection<Property> Properties => _properties;

    private PropertyType()
    {
    }

    public static PropertyType Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Название типа объекта обязательно.");
        }

        return new PropertyType
        {
            Name = name.Trim()
        };
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Название типа объекта обязательно.");
        }

        Name = name.Trim();
    }
}
