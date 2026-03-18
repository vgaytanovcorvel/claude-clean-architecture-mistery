using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MisteryApp.Repository.Ingest.Entities;

namespace MisteryApp.Repository.Ingest.Configurations;

public class MappedRecordEntityConfiguration : IEntityTypeConfiguration<MappedRecordEntity>
{
    public void Configure(EntityTypeBuilder<MappedRecordEntity> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.ExternalId).HasMaxLength(200);
        builder.Property(r => r.Description).HasMaxLength(2000);
        builder.Property(r => r.Source).HasMaxLength(500);
        builder.Property(r => r.Category).HasMaxLength(200);
        builder.Property(r => r.Value).HasColumnType("decimal(18,4)");
        builder.HasIndex(r => r.FileId);
        builder.HasIndex(r => r.IngestedAt);
    }
}
