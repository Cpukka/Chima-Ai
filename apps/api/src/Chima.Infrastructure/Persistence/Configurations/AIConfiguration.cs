using Chima.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chima.Infrastructure.Persistence.Configurations;

public class AIProviderConfiguration : IEntityTypeConfiguration<AIProvider>
{
    public void Configure(EntityTypeBuilder<AIProvider> b)
    {
        b.ToTable("ai_providers");
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).HasMaxLength(64).IsRequired();
        b.HasIndex(x => x.Name).IsUnique();
        b.Property(x => x.DisplayName).HasMaxLength(120);
        b.Property(x => x.BaseUrl).HasMaxLength(500);
        b.Property(x => x.ApiKeySecretName).HasMaxLength(120);
    }
}

public class AIModelConfiguration : IEntityTypeConfiguration<AIModel>
{
    public void Configure(EntityTypeBuilder<AIModel> b)
    {
        b.ToTable("ai_models");
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).HasMaxLength(64).IsRequired();
        b.HasIndex(x => x.Name).IsUnique();
        b.Property(x => x.DisplayName).HasMaxLength(120);
        b.Property(x => x.ModelIdentifier).HasMaxLength(120).IsRequired();
        b.Property(x => x.InputPricePer1M).HasPrecision(18, 6);
        b.Property(x => x.OutputPricePer1M).HasPrecision(18, 6);
        b.Property(x => x.CapabilitiesCsv).HasMaxLength(255);
        b.HasIndex(x => x.IsDefault);
        b.HasIndex(x => x.IsActive);

        b.HasOne(x => x.Provider).WithMany(p => p.Models)
            .HasForeignKey(x => x.ProviderId).OnDelete(DeleteBehavior.Restrict);
    }
}
