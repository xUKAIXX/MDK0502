using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateAgency.Domain;

namespace RealEstateAgency.Infrastructure.Persistence.Configurations;

public sealed class DealConfiguration : IEntityTypeConfiguration<Deal>
{
    public void Configure(EntityTypeBuilder<Deal> builder)
    {
        builder.ToTable("deals");
        builder.HasKey(deal => deal.Id);
        builder.Property(deal => deal.ClientName).HasMaxLength(100).IsRequired();
        builder.Property(deal => deal.ClientPhone).HasMaxLength(30).IsRequired();
        builder.Property(deal => deal.Type).HasConversion<int>();
        builder.Property(deal => deal.Status).HasConversion<int>();
        builder.Property(deal => deal.Amount)
            .HasConversion(
                value => (long)Math.Round(value * 100m),
                value => value / 100m)
            .HasColumnType("INTEGER");
        builder.Property(deal => deal.Comment).HasMaxLength(1000);
        builder.Property(deal => deal.CreatedAt).HasColumnType("TEXT");

        builder.HasOne(deal => deal.Property)
            .WithMany()
            .HasForeignKey(deal => deal.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(deal => deal.PropertyId);
        builder.HasIndex(deal => deal.Status);
    }
}
