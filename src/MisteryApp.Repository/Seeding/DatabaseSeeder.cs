using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MisteryApp.Repository.Contexts;
using MisteryApp.Repository.Entities;

namespace MisteryApp.Repository.Seeding;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
        await using var dbContext = await factory.CreateDbContextAsync();

        if (await dbContext.Users.AnyAsync())
            return;

        var alice = new UserEntity { Id = 1, Name = "Alice", AvatarUrl = "" };
        var bob = new UserEntity { Id = 2, Name = "Bob", AvatarUrl = "" };
        var charlie = new UserEntity { Id = 3, Name = "Charlie", AvatarUrl = "" };

        dbContext.Users.AddRange(alice, bob, charlie);

        var now = DateTime.UtcNow;

        dbContext.TodoItems.AddRange(
            new TodoItemEntity { Title = "Buy groceries", UserId = 1, CreatedAtUtc = now.AddHours(-5) },
            new TodoItemEntity { Title = "Read a book", UserId = 1, IsComplete = true, CreatedAtUtc = now.AddDays(-1), CompletedAtUtc = now.AddHours(-2) },
            new TodoItemEntity { Title = "Fix the leaky faucet", UserId = 2, CreatedAtUtc = now.AddHours(-3) },
            new TodoItemEntity { Title = "Plan weekend trip", UserId = 2, CreatedAtUtc = now.AddDays(-2) },
            new TodoItemEntity { Title = "Write blog post", UserId = 3, CreatedAtUtc = now.AddHours(-1) },
            new TodoItemEntity { Title = "Update resume", UserId = 3, IsComplete = true, CreatedAtUtc = now.AddDays(-3), CompletedAtUtc = now.AddDays(-1) }
        );

        await dbContext.SaveChangesAsync();
    }
}
