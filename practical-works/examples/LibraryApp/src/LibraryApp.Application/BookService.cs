using LibraryApp.Application.Contracts;
using LibraryApp.Domain;

namespace LibraryApp.Application;

public sealed class BookService(IBookRepository repository) : IBookService
{
    public async Task<IReadOnlyList<BookDto>> GetAllAsync(CancellationToken ct)
    {
        var books = await repository.GetAllAsync(ct);
        return books.Select(ToDto).ToList();
    }

    public async Task<BookDto?> GetByIdAsync(int id, CancellationToken ct)
    {
        var book = await repository.GetByIdAsync(id, ct);
        return book is null ? null : ToDto(book);
    }

    public async Task<BookDto> CreateAsync(CreateBookRequest request, CancellationToken ct)
    {
        var author = await repository.GetAuthorAsync(request.AuthorId, ct)
            ?? throw new DomainException("Автор не найден.");

        var book = Book.Create(request.Title, request.Year, author);

        if (request.GenreIds is { Count: > 0 })
        {
            var genres = await repository.GetGenresAsync(request.GenreIds, ct);

            foreach (var genre in genres)
            {
                book.AddGenre(genre);
            }
        }

        await repository.AddAsync(book, ct);
        await repository.SaveChangesAsync(ct);
        return ToDto(book);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var book = await repository.GetByIdAsync(id, ct)
            ?? throw new DomainException("Книга не найдена.");

        repository.Remove(book);
        await repository.SaveChangesAsync(ct);
    }

    private static BookDto ToDto(Book book) =>
        new(book.Id, book.Title, book.Year, book.Author.Name,
            book.Genres.Select(g => g.Name).ToList());
}
