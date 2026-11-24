using LibraryApp.Domain;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Book> Books => Set<Book>();

    public DbSet<Author> Authors => Set<Author>();

    public DbSet<Genre> Genres => Set<Genre>();

    public DbSet<Reader> Readers => Set<Reader>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
