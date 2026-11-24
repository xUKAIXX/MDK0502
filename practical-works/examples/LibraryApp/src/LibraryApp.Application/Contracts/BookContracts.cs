using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Application.Contracts;

public sealed record BookDto(
    int Id,
    string Title,
    int Year,
    string Author,
    IReadOnlyList<string> Genres);

public sealed record CreateBookRequest(
    [Required, StringLength(100, MinimumLength = 2)] string Title,
    [Range(1500, 2100)] int Year,
    [Range(1, int.MaxValue)] int AuthorId,
    IReadOnlyList<int>? GenreIds);
