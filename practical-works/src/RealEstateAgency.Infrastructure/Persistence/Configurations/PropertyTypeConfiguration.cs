using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateAgency.Domain;

namespace RealEstateAgency.Infrastructure.Persistence.Configurations;

public sealed class PropertyTypeConfiguration : IEntityTypeConfiguration<PropertyType>
{
    public void Configure(EntityTypeBuilder<PropertyType> builder)
    {
        builder.ToTable("property_types");
        builder.HasKey(propertyType => propertyType.Id);
        builder.Property(propertyType => propertyType.Name).HasMaxLength(100).IsRequired();
        builder.HasIndex(propertyType => propertyType.Name).IsUnique();
    }
}
