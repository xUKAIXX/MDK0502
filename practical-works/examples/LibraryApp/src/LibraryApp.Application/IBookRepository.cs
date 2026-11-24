using LibraryApp.Domain;

namespace LibraryApp.Application;

public interface IBookRepository
{
    Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken ct);

    Task<Book?> GetByIdAsync(int id, CancellationToken ct);

    Task<Author?> GetAuthorAsync(int id, CancellationToken ct);

    Task<IReadOnlyList<Genre>> GetGenresAsync(IReadOnlyCollection<int> ids, CancellationToken ct);

    Task AddAsync(Book book, CancellationToken ct);

    void Remove(Book book);

    Task SaveChangesAsync(CancellationToken ct);
}
