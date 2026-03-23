using Microsoft.EntityFrameworkCore;
using MisteryApp.Repository.Contexts;

namespace MisteryApp.Repository.Tests.Helpers;

public class TestDbContextFactory(string databaseName) : IDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new ApplicationDbContext(options);
    }

    public Task<ApplicationDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(CreateDbContext());
    }
}
