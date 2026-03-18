using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MisteryApp.Repository.Ingest.Entities;

namespace MisteryApp.Repository.Ingest.Configurations;

public class IngestFileEntityConfiguration : IEntityTypeConfiguration<IngestFileEntity>
{
    public void Configure(EntityTypeBuilder<IngestFileEntity> builder)
    {
        builder.ToTable("IngestFiles");
        builder.HasKey(f => f.Id);
        builder.Property(f => f.FilePath).IsRequired().HasMaxLength(2000);
        builder.Property(f => f.RunId).IsRequired();
        builder.HasIndex(f => f.RunId);
        builder.HasOne(f => f.RejectedFile)
            .WithOne(rf => rf.File)
            .HasForeignKey<IngestRejectedFileEntity>(rf => rf.FileId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(f => f.MappedRecords)
            .WithOne(r => r.File)
            .HasForeignKey(r => r.FileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
