namespace LibraryApp.Domain;

public class Author
{
    public int Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? Country { get; private set; }

    private readonly List<Book> _books = new();

    public IReadOnlyCollection<Book> Books => _books;

    private Author()
    {
    }

    public static Author Create(string name, string? country)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Имя автора обязательно.");
        }

        return new Author { Name = name.Trim(), Country = country?.Trim() };
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Имя автора обязательно.");
        }

        Name = name.Trim();
    }
}
