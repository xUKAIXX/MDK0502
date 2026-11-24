using LibraryApp.Application.Contracts;
using LibraryApp.Domain;

namespace LibraryApp.Application;

public sealed class ReaderService(IReaderRepository readers, IBookRepository books) : IReaderService
{
    public async Task<IReadOnlyList<ReaderDto>> GetAllAsync(CancellationToken ct)
    {
        var all = await readers.GetAllAsync(ct);
        return all.Select(ToDto).ToList();
    }

    public async Task<ReaderDto?> GetByIdAsync(int id, CancellationToken ct)
    {
        var reader = await readers.GetByIdAsync(id, ct);
        return reader is null ? null : ToDto(reader);
    }

    public async Task<ReaderDto> CreateAsync(CreateReaderRequest request, CancellationToken ct)
    {
        var reader = Reader.Create(request.Name, request.Email, DateTime.UtcNow);

        await readers.AddAsync(reader, ct);
        await readers.SaveChangesAsync(ct);

        return ToDto(reader);
    }

    public async Task GiveBookAsync(int readerId, int bookId, CancellationToken ct)
    {
        var reader = await readers.GetByIdAsync(readerId, ct)
            ?? throw new DomainException("Читатель не найден.");

        var book = await books.GetByIdAsync(bookId, ct)
            ?? throw new DomainException("Книга не найдена.");

        // Связь «многие ко многим»: книга добавляется читателю и наоборот.
        reader.TakeBook(book);
        book.GiveToReader(reader);

        await readers.SaveChangesAsync(ct);
    }

    private static ReaderDto ToDto(Reader reader) =>
        new(reader.Id, reader.Name, reader.Email, reader.RegisteredAt,
            reader.Books.Select(b => b.Title).ToList());
}
