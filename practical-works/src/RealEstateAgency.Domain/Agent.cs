namespace RealEstateAgency.Domain;

public class Agent
{
    private readonly List<Property> _properties = new();

    public int Id { get; private set; }

    public string FullName { get; private set; } = string.Empty;

    public string Phone { get; private set; } = string.Empty;

    public string? Email { get; private set; }

    public int ExperienceYears { get; private set; }

    public IReadOnlyCollection<Property> Properties => _properties;

    private Agent()
    {
    }

    public static Agent Create(string fullName, string phone, string? email, int experienceYears)
    {
        Validate(fullName, phone, experienceYears);

        return new Agent
        {
            FullName = fullName.Trim(),
            Phone = phone.Trim(),
            Email = NormalizeEmail(email),
            ExperienceYears = experienceYears
        };
    }

    public void Update(string fullName, string phone, string? email, int experienceYears)
    {
        Validate(fullName, phone, experienceYears);

        FullName = fullName.Trim();
        Phone = phone.Trim();
        Email = NormalizeEmail(email);
        ExperienceYears = experienceYears;
    }

    private static void Validate(string fullName, string phone, int experienceYears)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new DomainException("ФИО агента обязательно.");
        }

        if (fullName.Trim().Length is < 2 or > 100)
        {
            throw new DomainException("ФИО агента должно быть от 2 до 100 символов.");
        }

        if (string.IsNullOrWhiteSpace(phone))
        {
            throw new DomainException("Телефон агента обязателен.");
        }

        if (phone.Trim().Length is < 5 or > 30)
        {
            throw new DomainException("Телефон агента должен быть от 5 до 30 символов.");
        }

        if (experienceYears < 0)
        {
            throw new DomainException("Стаж агента не может быть отрицательным.");
        }
    }

    private static string? NormalizeEmail(string? email)
    {
        return string.IsNullOrWhiteSpace(email) ? null : email.Trim();
    }
}
