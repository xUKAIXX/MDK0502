namespace LibraryApp.Domain;

public class Genre
{
    public int Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    private readonly List<Book> _books = new();

    public IReadOnlyCollection<Book> Books => _books;

    private Genre()
    {
    }

    public static Genre Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Название жанра обязательно.");
        }

        return new Genre { Name = name.Trim() };
    }
}
