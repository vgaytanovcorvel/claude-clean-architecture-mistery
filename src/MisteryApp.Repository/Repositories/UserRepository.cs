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
    public virtual async Task<User> UserSingleByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await UserSingleOrDefaultByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"User not found (UserId: {id}).");
    }

    public virtual async Task<User?> UserSingleOrDefaultByIdAsync(int id, CancellationToken cancellationToken)
    {
        await using var dbContext = await CreateContextAsync(cancellationToken);

        var entity = await dbContext.User
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserId == id, cancellationToken);

        return entity is null ? null : MapToDomain(entity);
    }

    public virtual async Task<User?> UserSingleOrDefaultByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        await using var dbContext = await CreateContextAsync(cancellationToken);

        var entity = await dbContext.User
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);

        return entity is null ? null : MapToDomain(entity);
    }

    public virtual async Task<User> UserAddAsync(User user, CancellationToken cancellationToken)
    {
        await using var dbContext = await CreateContextAsync(cancellationToken);

        var entity = MapToEntity(user);
        var entry = await dbContext.User.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToDomain(entry.Entity);
    }

    private static User MapToDomain(UserEntity entity)
    {
        return new User
        {
            UserId = entity.UserId,
            Username = entity.Username,
            DisplayName = entity.DisplayName,
            CreatedAt = entity.CreatedAt
        };
    }

    private static UserEntity MapToEntity(User user)
    {
        return new UserEntity
        {
            UserId = user.UserId,
            Username = user.Username,
            DisplayName = user.DisplayName,
            CreatedAt = user.CreatedAt
        };
    }
}
