using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MisteryApp.Repository.Ingest.Entities;

namespace MisteryApp.Repository.Ingest.Configurations;

public class IngestRunEntityConfiguration : IEntityTypeConfiguration<IngestRunEntity>
{
    public void Configure(EntityTypeBuilder<IngestRunEntity> builder)
    {
        builder.ToTable("IngestRuns");
        builder.HasKey(r => r.RunId);
        builder.Property(r => r.InputPath).IsRequired().HasMaxLength(2000);
        builder.Property(r => r.Status).HasConversion<int>().HasColumnType("int");
        builder.HasIndex(r => r.StartedAt);
        builder.HasMany(r => r.Files)
            .WithOne(f => f.Run)
            .HasForeignKey(f => f.RunId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
