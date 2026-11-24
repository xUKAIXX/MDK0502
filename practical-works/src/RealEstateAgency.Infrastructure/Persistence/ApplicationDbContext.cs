using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealEstateAgency.Domain;
using RealEstateAgency.Infrastructure.Identity;

namespace RealEstateAgency.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Property> Properties => Set<Property>();

    public DbSet<PropertyType> PropertyTypes => Set<PropertyType>();

    public DbSet<District> Districts => Set<District>();

    public DbSet<Agent> Agents => Set<Agent>();

    public DbSet<Deal> Deals => Set<Deal>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(user => user.FullName).HasMaxLength(100).IsRequired();
            entity.Property(user => user.Phone).HasMaxLength(30);
            entity.Property(user => user.RegisteredAt).HasColumnType("TEXT");
        });

        modelBuilder.Entity<Property>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(property => property.OwnerId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
