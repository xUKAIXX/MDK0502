using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateAgency.Domain;

namespace RealEstateAgency.Infrastructure.Persistence.Configurations;

public sealed class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.ToTable("properties");
        builder.HasKey(property => property.Id);

        builder.Property(property => property.Title).HasMaxLength(150).IsRequired();
        builder.Property(property => property.Address).HasMaxLength(200).IsRequired();
        builder.Property(property => property.Price)
            .HasConversion(
                value => (long)Math.Round(value * 100m),
                value => value / 100m)
            .HasColumnType("INTEGER");
        builder.Property(property => property.Area).HasColumnType("REAL");
        builder.Property(property => property.Description).HasMaxLength(2000);
        builder.Property(property => property.Status).HasConversion<int>();
        builder.Property(property => property.OwnerId).HasMaxLength(450);
        builder.Property(property => property.OwnerName).HasMaxLength(100);
        builder.Property(property => property.CreatedAt).HasColumnType("TEXT");

        builder.HasOne(property => property.PropertyType)
            .WithMany(propertyType => propertyType.Properties)
            .HasForeignKey(property => property.PropertyTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(property => property.District)
            .WithMany(district => district.Properties)
            .HasForeignKey(property => property.DistrictId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(property => property.Agent)
            .WithMany(agent => agent.Properties)
            .HasForeignKey(property => property.AgentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(property => property.Status);
        builder.HasIndex(property => property.Price);
        builder.HasIndex(property => property.PropertyTypeId);
        builder.HasIndex(property => property.DistrictId);
        builder.HasIndex(property => property.AgentId);
        builder.HasIndex(property => property.OwnerId);
    }
}
