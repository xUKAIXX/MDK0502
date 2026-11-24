using LibraryApp.Application.Contracts;

namespace LibraryApp.Application;

public interface IReaderService
{
    Task<IReadOnlyList<ReaderDto>> GetAllAsync(CancellationToken ct);

    Task<ReaderDto?> GetByIdAsync(int id, CancellationToken ct);

    Task<ReaderDto> CreateAsync(CreateReaderRequest request, CancellationToken ct);

    Task GiveBookAsync(int readerId, int bookId, CancellationToken ct);
}
