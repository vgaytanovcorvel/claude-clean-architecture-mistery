using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MisteryApp.Repository.Entities;

namespace MisteryApp.Repository.Configurations;

public class TodoItemEntityConfiguration : IEntityTypeConfiguration<TodoItemEntity>
{
    public void Configure(EntityTypeBuilder<TodoItemEntity> builder)
    {
        builder.HasKey(e => e.TodoItemId);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasColumnType("NVARCHAR(200)");

        builder.Property(e => e.Description)
            .HasColumnType("NVARCHAR(MAX)");

        builder.HasOne(e => e.User)
            .WithMany(u => u.TodoItems)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.UserId);
    }
}
