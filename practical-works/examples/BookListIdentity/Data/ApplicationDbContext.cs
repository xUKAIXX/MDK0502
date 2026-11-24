using BookList.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookList.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Book> Books => Set<Book>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Обязательный вызов: добавляет в модель таблицы Identity.
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(user => user.FullName).HasMaxLength(100);
            entity.Property(user => user.RegisteredAt).HasColumnType("TEXT");
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.ToTable("books");
            entity.HasKey(book => book.Id);
            entity.Property(book => book.Id).ValueGeneratedOnAdd();
            entity.Property(book => book.Title).HasMaxLength(100).IsRequired();
            entity.Property(book => book.Author).HasMaxLength(60).IsRequired();
            entity.Property(book => book.Genre).HasMaxLength(40).IsRequired();
            entity.HasIndex(book => book.OwnerId).HasDatabaseName("ix_books_owner_id");

            entity.HasOne(book => book.Owner)
                .WithMany(user => user.Books)
                .HasForeignKey(book => book.OwnerId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
