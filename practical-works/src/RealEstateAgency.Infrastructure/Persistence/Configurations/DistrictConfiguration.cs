using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateAgency.Domain;

namespace RealEstateAgency.Infrastructure.Persistence.Configurations;

public sealed class DistrictConfiguration : IEntityTypeConfiguration<District>
{
    public void Configure(EntityTypeBuilder<District> builder)
    {
        builder.ToTable("districts");
        builder.HasKey(district => district.Id);
        builder.Property(district => district.Name).HasMaxLength(100).IsRequired();
        builder.Property(district => district.City).HasMaxLength(100).IsRequired();
        builder.HasIndex(district => new { district.Name, district.City }).IsUnique();
    }
}
