using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateAgency.Domain;

namespace RealEstateAgency.Infrastructure.Persistence.Configurations;

public sealed class AgentConfiguration : IEntityTypeConfiguration<Agent>
{
    public void Configure(EntityTypeBuilder<Agent> builder)
    {
        builder.ToTable("agents");
        builder.HasKey(agent => agent.Id);
        builder.Property(agent => agent.FullName).HasMaxLength(100).IsRequired();
        builder.Property(agent => agent.Phone).HasMaxLength(30).IsRequired();
        builder.Property(agent => agent.Email).HasMaxLength(100);
        builder.HasIndex(agent => agent.Phone).IsUnique();
    }
}
