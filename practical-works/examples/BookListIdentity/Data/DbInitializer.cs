using BookList.Models;
using Microsoft.EntityFrameworkCore;

namespace BookList.Data;

/// <summary>
/// Начальное наполнение каталога книг. Книги из сида не имеют владельца:
/// владелец проставляется только у книг, созданных в приложении.
/// </summary>
public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        context.Database.EnsureCreated();

        if (context.Books.Any())
        {
            return;
        }

        var books = new[]
        {
            new Book { Title = "Война и мир", Author = "Толстой", Year = 1869, Genre = "Роман" },
            new Book { Title = "Преступление и наказание", Author = "Достоевский", Year = 1866, Genre = "Роман" },
            new Book { Title = "Мастер и Маргарита", Author = "Булгаков", Year = 1967, Genre = "Фантастика" },
            new Book { Title = "Отцы и дети", Author = "Тургенев", Year = 1862, Genre = "Роман" },
            new Book { Title = "Собачье сердце", Author = "Булгаков", Year = 1925, Genre = "Повесть" },
            new Book { Title = "Евгений Онегин", Author = "Пушкин", Year = 1833, Genre = "Поэзия" },
            new Book { Title = "Тихий Дон", Author = "Шолохов", Year = 1940, Genre = "Роман" },
            new Book { Title = "Два капитана", Author = "Каверин", Year = 1944, Genre = "Приключения" }
        };

        context.Books.AddRange(books);
        context.SaveChanges();
    }
}
