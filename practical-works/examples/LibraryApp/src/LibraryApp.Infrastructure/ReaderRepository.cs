using LibraryApp.Application;
using LibraryApp.Domain;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Infrastructure;

public sealed class ReaderRepository(AppDbContext context) : IReaderRepository
{
    public async Task<IReadOnlyList<Reader>> GetAllAsync(CancellationToken ct)
        => await context.Readers
            .Include(x => x.Books)
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(ct);

    public async Task<Reader?> GetByIdAsync(int id, CancellationToken ct)
        => await context.Readers
            .Include(x => x.Books)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(Reader reader, CancellationToken ct)
        => await context.Readers.AddAsync(reader, ct);

    public Task SaveChangesAsync(CancellationToken ct) => context.SaveChangesAsync(ct);
}
