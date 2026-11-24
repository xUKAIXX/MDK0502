using LibraryApp.Application;
using LibraryApp.Infrastructure;
using LibraryApp.Domain;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IReaderService, ReaderService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Создаём схему и наполняем базу демонстрационными данными.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();

    if (!context.Authors.Any())
    {
        var tolstoy = Author.Create("Толстой", "Россия");
        var dostoevsky = Author.Create("Достоевский", "Россия");
        var bulgakov = Author.Create("Булгаков", "Россия");

        var novel = Genre.Create("Роман");
        var fantasy = Genre.Create("Фантастика");

        context.Authors.AddRange(tolstoy, dostoevsky, bulgakov);
        context.Genres.AddRange(novel, fantasy);
        context.SaveChanges();

        var warAndPeace = Book.Create("Война и мир", 1869, tolstoy);
        var crime = Book.Create("Преступление и наказание", 1866, dostoevsky);
        var master = Book.Create("Мастер и Маргарита", 1967, bulgakov);

        warAndPeace.AddGenre(novel);
        crime.AddGenre(novel);
        master.AddGenre(fantasy);

        context.Books.AddRange(warAndPeace, crime, master);

        context.Readers.AddRange(
            Reader.Create("Иван Иванов", "ivanov@example.com", DateTime.UtcNow.AddYears(-1)),
            Reader.Create("Мария Петрова", "petrova@example.com", DateTime.UtcNow));

        context.SaveChanges();
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
