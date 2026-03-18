using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MisteryApp.Repository.Entities;

namespace MisteryApp.Repository.Configuration;

public class IngestedRecordEntityConfiguration : IEntityTypeConfiguration<IngestedRecordEntity>
{
    public void Configure(EntityTypeBuilder<IngestedRecordEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.SourceFile).IsRequired().HasMaxLength(1000);
        builder.Property(e => e.Format).IsRequired().HasMaxLength(50);
        builder.Property(e => e.FieldsJson).IsRequired().HasColumnType("nvarchar(max)");
        builder.Property(e => e.HealingNotes).HasMaxLength(4000);
        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Property(e => e.RunId).IsRequired();
    }
}
