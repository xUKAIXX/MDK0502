namespace LibraryApp.Domain;

public class Book
{
    public int Id { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public int Year { get; private set; }

    public int AuthorId { get; private set; }

    public Author Author { get; private set; } = null!;

    private readonly List<Genre> _genres = new();

    public IReadOnlyCollection<Genre> Genres => _genres;

    private readonly List<Reader> _readers = new();

    public IReadOnlyCollection<Reader> Readers => _readers;

    private Book()
    {
    }

    public static Book Create(string title, int year, Author author)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("Название обязательно.");
        }

        if (year is < 1500 or > 2100)
        {
            throw new DomainException("Год должен быть в диапазоне 1500–2100.");
        }

        if (author is null)
        {
            throw new DomainException("Автор обязателен.");
        }

        return new Book { Title = title.Trim(), Year = year, AuthorId = author.Id, Author = author };
    }

    public void AddGenre(Genre genre)
    {
        if (genre is null)
        {
            throw new DomainException("Жанр обязателен.");
        }

        if (_genres.All(g => g.Id != genre.Id))
        {
            _genres.Add(genre);
        }
    }

    public void GiveToReader(Reader reader)
    {
        if (reader is null)
        {
            throw new DomainException("Читатель обязателен.");
        }

        if (_readers.All(r => r.Id != reader.Id))
        {
            _readers.Add(reader);
        }
    }
}
