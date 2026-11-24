using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Application.Contracts;

public sealed record ReaderDto(
    int Id,
    string Name,
    string? Email,
    DateTime RegisteredAt,
    IReadOnlyList<string> Books);

public sealed record CreateReaderRequest(
    [Required, StringLength(100, MinimumLength = 2)] string Name,
    [EmailAddress, StringLength(100)] string? Email);

public sealed record GiveBookRequest([Range(1, int.MaxValue)] int BookId);
