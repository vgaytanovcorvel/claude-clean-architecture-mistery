using Microsoft.EntityFrameworkCore;
using MisteryApp.Abstractions.Exceptions;
using MisteryApp.Abstractions.Interfaces;
using MisteryApp.Abstractions.Models;
using MisteryApp.Repository.Contexts;
using MisteryApp.Repository.Entities;

namespace MisteryApp.Repository.Repositories;

public class TodoItemRepository(
    IDbContextFactory<ApplicationDbContext> contextFactory)
    : RepositoryBase<ApplicationDbContext>(contextFactory), ITodoItemRepository
{
    public virtual async Task<TodoItem> TodoItemSingleByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await TodoItemSingleOrDefaultByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"TodoItem not found (TodoItemId: {id}).");
    }

    public virtual async Task<TodoItem?> TodoItemSingleOrDefaultByIdAsync(int id, CancellationToken cancellationToken)
    {
        await using var dbContext = await CreateContextAsync(cancellationToken);

        var entity = await dbContext.TodoItem
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.TodoItemId == id, cancellationToken);

        return entity is null ? null : MapToDomain(entity);
    }

    public virtual async Task<IReadOnlyList<TodoItem>> TodoItemGetAllByUserIdAsync(int userId, CancellationToken cancellationToken)
    {
        await using var dbContext = await CreateContextAsync(cancellationToken);

        var entities = await dbContext.TodoItem
            .AsNoTracking()
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);

        return entities.Select(MapToDomain).ToList();
    }

    public virtual async Task<TodoItem> TodoItemAddAsync(TodoItem item, CancellationToken cancellationToken)
    {
        await using var dbContext = await CreateContextAsync(cancellationToken);

        var entity = MapToEntity(item);
        var entry = await dbContext.TodoItem.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToDomain(entry.Entity);
    }

    public virtual async Task<TodoItem> TodoItemUpdateAsync(TodoItem item, CancellationToken cancellationToken)
    {
        await using var dbContext = await CreateContextAsync(cancellationToken);

        var entity = MapToEntity(item);
        dbContext.TodoItem.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToDomain(entity);
    }

    public virtual async Task TodoItemDeleteAsync(int id, CancellationToken cancellationToken)
    {
        await using var dbContext = await CreateContextAsync(cancellationToken);

        var entity = await dbContext.TodoItem
            .FirstOrDefaultAsync(t => t.TodoItemId == id, cancellationToken)
            ?? throw new NotFoundException($"TodoItem not found (TodoItemId: {id}).");

        dbContext.TodoItem.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static TodoItem MapToDomain(TodoItemEntity entity)
    {
        return new TodoItem
        {
            TodoItemId = entity.TodoItemId,
            UserId = entity.UserId,
            Title = entity.Title,
            Description = entity.Description,
            IsCompleted = entity.IsCompleted,
            CreatedAt = entity.CreatedAt,
            CompletedAt = entity.CompletedAt
        };
    }

    private static TodoItemEntity MapToEntity(TodoItem item)
    {
        return new TodoItemEntity
        {
            TodoItemId = item.TodoItemId,
            UserId = item.UserId,
            Title = item.Title,
            Description = item.Description,
            IsCompleted = item.IsCompleted,
            CreatedAt = item.CreatedAt,
            CompletedAt = item.CompletedAt
        };
    }
}
