using LibraryApp.Application.Contracts;

namespace LibraryApp.Application;

public interface IBookService
{
    Task<IReadOnlyList<BookDto>> GetAllAsync(CancellationToken ct);

    Task<BookDto?> GetByIdAsync(int id, CancellationToken ct);

    Task<BookDto> CreateAsync(CreateBookRequest request, CancellationToken ct);

    Task DeleteAsync(int id, CancellationToken ct);
}
