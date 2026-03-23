using Microsoft.EntityFrameworkCore;
using MisteryApp.Repository.Entities;

namespace MisteryApp.Repository.Contexts;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<UserEntity> User => Set<UserEntity>();
    public DbSet<TodoItemEntity> TodoItem => Set<TodoItemEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
