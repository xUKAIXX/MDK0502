using LibraryApp.Application;
using LibraryApp.Domain;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Infrastructure;

public sealed class BookRepository(AppDbContext context) : IBookRepository
{
    public async Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken ct)
        => await context.Books
            .Include(x => x.Author)
            .Include(x => x.Genres)
            .AsNoTracking()
            .OrderBy(x => x.Title)
            .ToListAsync(ct);

    public async Task<Book?> GetByIdAsync(int id, CancellationToken ct)
        => await context.Books
            .Include(x => x.Author)
            .Include(x => x.Genres)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<Author?> GetAuthorAsync(int id, CancellationToken ct)
        => await context.Authors.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<Genre>> GetGenresAsync(IReadOnlyCollection<int> ids, CancellationToken ct)
        => await context.Genres
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(ct);

    public async Task AddAsync(Book book, CancellationToken ct)
        => await context.Books.AddAsync(book, ct);

    public void Remove(Book book) => context.Books.Remove(book);

    public Task SaveChangesAsync(CancellationToken ct) => context.SaveChangesAsync(ct);
}
