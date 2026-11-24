namespace LibraryApp.Domain;

/// <summary>
/// Индивидуальное задание: сущность «Читатель» со связью «многие ко многим» с книгами.
/// </summary>
public class Reader
{
    public int Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? Email { get; private set; }

    public DateTime RegisteredAt { get; private set; }

    private readonly List<Book> _books = new();

    public IReadOnlyCollection<Book> Books => _books;

    private Reader()
    {
    }

    public static Reader Create(string name, string? email, DateTime registeredAt)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Имя читателя обязательно.");
        }

        if (name.Trim().Length is < 2 or > 100)
        {
            throw new DomainException("Имя читателя должно быть от 2 до 100 символов.");
        }

        return new Reader
        {
            Name = name.Trim(),
            Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim(),
            RegisteredAt = registeredAt
        };
    }

    public void TakeBook(Book book)
    {
        if (book is null)
        {
            throw new DomainException("Книга обязательна.");
        }

        if (_books.All(b => b.Id != book.Id))
        {
            _books.Add(book);
        }
    }
}
