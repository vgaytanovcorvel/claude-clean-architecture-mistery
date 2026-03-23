using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MisteryApp.Repository.Entities;

namespace MisteryApp.Repository.Configurations;

public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(e => e.UserId);

        builder.Property(e => e.Username)
            .IsRequired()
            .HasColumnType("VARCHAR(50)");

        builder.HasIndex(e => e.Username)
            .IsUnique();

        builder.Property(e => e.DisplayName)
            .IsRequired()
            .HasColumnType("NVARCHAR(100)");

        builder.Property(e => e.CreatedAt)
            .IsRequired();
    }
}
