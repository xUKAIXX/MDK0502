using LibraryApp.Domain;
using Xunit;

namespace LibraryApp.Tests;

public sealed class BookTests
{
    [Theory]
    [InlineData(1499)]
    [InlineData(2101)]
    public void Create_WithYearOutOfRange_Throws(int year)
    {
        var author = Author.Create("Толстой", "Россия");

        Assert.Throws<DomainException>(() => Book.Create("Война и мир", year, author));
    }

    [Fact]
    public void Create_WithoutAuthor_Throws()
    {
        Assert.Throws<DomainException>(() => Book.Create("Война и мир", 1869, null!));
    }

    [Fact]
    public void AddGenre_DoesNotDuplicateGenre()
    {
        var book = Book.Create("Война и мир", 1869, Author.Create("Толстой", "Россия"));
        var genre = Genre.Create("Роман");

        book.AddGenre(genre);
        book.AddGenre(genre);

        Assert.Single(book.Genres);
    }
}
