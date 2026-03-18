using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MisteryApp.Repository.Ingest.Entities;

namespace MisteryApp.Repository.Ingest.Configurations;

public class IngestRejectedFileEntityConfiguration : IEntityTypeConfiguration<IngestRejectedFileEntity>
{
    public void Configure(EntityTypeBuilder<IngestRejectedFileEntity> builder)
    {
        builder.ToTable("IngestRejectedFiles");
        builder.HasKey(rf => rf.Id);
        builder.Property(rf => rf.FilePath).IsRequired().HasMaxLength(2000);
        builder.Property(rf => rf.RejectionReason).IsRequired().HasMaxLength(4000);
        builder.Property(rf => rf.RunId).IsRequired();
        builder.HasIndex(rf => rf.RunId);
    }
}
