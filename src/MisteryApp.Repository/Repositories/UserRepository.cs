using Microsoft.EntityFrameworkCore;
using MisteryApp.Abstractions.Exceptions;
using MisteryApp.Abstractions.Interfaces;
using MisteryApp.Abstractions.Models;
using MisteryApp.Repository.Contexts;
using MisteryApp.Repository.Entities;

namespace MisteryApp.Repository.Repositories;

public class UserRepository(
    IDbContextFactory<ApplicationDbContext> contextFactory)
    : RepositoryBase<ApplicationDbContext>(contextFactory), IUserRepository
{
    public virtual async Task<IReadOnlyList<User>> UserGetAllAsync(CancellationToken cancellationToken)
    {
        await using var dbContext = await CreateContextAsync(cancellationToken);

        var entities = await dbContext.Users
            .AsNoTracking()
            .OrderBy(u => u.Name)
            .ToListAsync(cancellationToken);

        return entities.Select(MapToDomain).ToList();
    }

    public virtual async Task<User> UserSingleByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await UserSingleOrDefaultByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"User not found (UserId: {id}).");
    }

    public virtual async Task<User?> UserSingleOrDefaultByIdAsync(int id, CancellationToken cancellationToken)
    {
        await using var dbContext = await CreateContextAsync(cancellationToken);

        var entity = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        return entity is null ? null : MapToDomain(entity);
    }

    private static User MapToDomain(UserEntity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        AvatarUrl = entity.AvatarUrl,
    };
}
