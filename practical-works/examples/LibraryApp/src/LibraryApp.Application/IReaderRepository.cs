using LibraryApp.Domain;

namespace LibraryApp.Application;

public interface IReaderRepository
{
    Task<IReadOnlyList<Reader>> GetAllAsync(CancellationToken ct);

    Task<Reader?> GetByIdAsync(int id, CancellationToken ct);

    Task AddAsync(Reader reader, CancellationToken ct);

    Task SaveChangesAsync(CancellationToken ct);
}
