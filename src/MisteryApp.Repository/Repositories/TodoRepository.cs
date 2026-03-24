using Microsoft.EntityFrameworkCore;
using MisteryApp.Abstractions.Exceptions;
using MisteryApp.Abstractions.Interfaces;
using MisteryApp.Abstractions.Models;
using MisteryApp.Repository.Contexts;
using MisteryApp.Repository.Entities;

namespace MisteryApp.Repository.Repositories;

public class TodoRepository(
    IDbContextFactory<ApplicationDbContext> contextFactory)
    : RepositoryBase<ApplicationDbContext>(contextFactory), ITodoRepository
{
    public virtual async Task<IReadOnlyList<TodoItem>> TodoFindByUserIdAsync(int userId, CancellationToken cancellationToken)
    {
        await using var dbContext = await CreateContextAsync(cancellationToken);

        var entities = await dbContext.TodoItems
            .AsNoTracking()
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return entities.Select(MapToDomain).ToList();
    }

    public virtual async Task<TodoItem> TodoSingleByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await TodoSingleOrDefaultByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Todo item not found (TodoItemId: {id}).");
    }

    public virtual async Task<TodoItem> TodoAddAsync(TodoItem todoItem, CancellationToken cancellationToken)
    {
        await using var dbContext = await CreateContextAsync(cancellationToken);

        var entity = MapToEntity(todoItem);
        var entry = await dbContext.TodoItems.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return MapToDomain(entry.Entity);
    }

    public virtual async Task<TodoItem> TodoUpdateAsync(TodoItem todoItem, CancellationToken cancellationToken)
    {
        await using var dbContext = await CreateContextAsync(cancellationToken);

        var entity = MapToEntity(todoItem);
        dbContext.TodoItems.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return MapToDomain(entity);
    }

    public virtual async Task TodoDeleteAsync(int id, CancellationToken cancellationToken)
    {
        await using var dbContext = await CreateContextAsync(cancellationToken);

        var entity = await dbContext.TodoItems
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Todo item not found (TodoItemId: {id}).");

        dbContext.TodoItems.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<TodoItem?> TodoSingleOrDefaultByIdAsync(int id, CancellationToken cancellationToken)
    {
        await using var dbContext = await CreateContextAsync(cancellationToken);

        var entity = await dbContext.TodoItems
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        return entity is null ? null : MapToDomain(entity);
    }

    private static TodoItem MapToDomain(TodoItemEntity entity) => new()
    {
        Id = entity.Id,
        Title = entity.Title,
        IsComplete = entity.IsComplete,
        UserId = entity.UserId,
        CreatedAtUtc = entity.CreatedAtUtc,
        CompletedAtUtc = entity.CompletedAtUtc,
    };

    private static TodoItemEntity MapToEntity(TodoItem todoItem) => new()
    {
        Id = todoItem.Id,
        Title = todoItem.Title,
        IsComplete = todoItem.IsComplete,
        UserId = todoItem.UserId,
        CreatedAtUtc = todoItem.CreatedAtUtc,
        CompletedAtUtc = todoItem.CompletedAtUtc,
    };
}
