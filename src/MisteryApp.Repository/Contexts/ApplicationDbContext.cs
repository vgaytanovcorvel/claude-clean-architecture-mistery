using Microsoft.EntityFrameworkCore;
using MisteryApp.Repository.Ingest.Entities;

namespace MisteryApp.Repository.Contexts;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<IngestRunEntity> IngestRuns => Set<IngestRunEntity>();
    public DbSet<IngestRejectedFileEntity> IngestRejectedFiles => Set<IngestRejectedFileEntity>();
    public DbSet<MappedRecordEntity> MappedRecords => Set<MappedRecordEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
