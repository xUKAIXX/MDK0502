using LibraryApp.Domain;
using Xunit;

namespace LibraryApp.Tests;

public sealed class ReaderTests
{
    [Fact]
    public void Create_WithShortName_Throws()
    {
        Assert.Throws<DomainException>(() => Reader.Create("А", "a@example.com", DateTime.UtcNow));
    }

    [Fact]
    public void TakeBook_AddsBookOnce()
    {
        var reader = Reader.Create("Иван Иванов", "ivanov@example.com", DateTime.UtcNow);
        var book = Book.Create("Война и мир", 1869, Author.Create("Толстой", "Россия"));

        reader.TakeBook(book);
        reader.TakeBook(book);

        Assert.Single(reader.Books);
    }
}
