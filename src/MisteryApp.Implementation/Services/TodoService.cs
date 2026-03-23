using MisteryApp.Abstractions.Interfaces;
using MisteryApp.Abstractions.Models;
using MisteryApp.Abstractions.Models.Requests;

namespace MisteryApp.Implementation.Services;

public class TodoService(
    ITodoItemRepository todoItemRepository,
    TimeProvider timeProvider) : ITodoService
{
    public virtual async Task<IReadOnlyList<TodoItem>> GetTodosByUserIdAsync(int userId, CancellationToken cancellationToken)
    {
        return await todoItemRepository.TodoItemGetAllByUserIdAsync(userId, cancellationToken);
    }

    public virtual async Task<TodoItem> CreateTodoAsync(int userId, CreateTodoRequest request, CancellationToken cancellationToken)
    {
        var todoItem = new TodoItem
        {
            UserId = userId,
            Title = request.Title,
            Description = request.Description,
            IsCompleted = false,
            CreatedAt = timeProvider.GetUtcNow()
        };

        return await todoItemRepository.TodoItemAddAsync(todoItem, cancellationToken);
    }

    public virtual async Task<TodoItem> UpdateTodoAsync(int todoItemId, int userId, UpdateTodoRequest request, CancellationToken cancellationToken)
    {
        var todo = await todoItemRepository.TodoItemSingleByIdAsync(todoItemId, cancellationToken);
        ValidateOwnership(todo, userId);

        var wasCompleted = todo.IsCompleted;
        var completedAt = DetermineCompletedAt(wasCompleted, request.IsCompleted, todo.CompletedAt);

        var updated = new TodoItem
        {
            TodoItemId = todo.TodoItemId,
            UserId = todo.UserId,
            Title = request.Title,
            Description = request.Description,
            IsCompleted = request.IsCompleted,
            CreatedAt = todo.CreatedAt,
            CompletedAt = completedAt
        };

        return await todoItemRepository.TodoItemUpdateAsync(updated, cancellationToken);
    }

    public virtual async Task DeleteTodoAsync(int todoItemId, int userId, CancellationToken cancellationToken)
    {
        var todo = await todoItemRepository.TodoItemSingleByIdAsync(todoItemId, cancellationToken);
        ValidateOwnership(todo, userId);

        await todoItemRepository.TodoItemDeleteAsync(todoItemId, cancellationToken);
    }

    public virtual async Task<TodoItem> ToggleTodoAsync(int todoItemId, int userId, CancellationToken cancellationToken)
    {
        var todo = await todoItemRepository.TodoItemSingleByIdAsync(todoItemId, cancellationToken);
        ValidateOwnership(todo, userId);

        var toggled = !todo.IsCompleted;

        var updated = new TodoItem
        {
            TodoItemId = todo.TodoItemId,
            UserId = todo.UserId,
            Title = todo.Title,
            Description = todo.Description,
            IsCompleted = toggled,
            CreatedAt = todo.CreatedAt,
            CompletedAt = toggled ? timeProvider.GetUtcNow() : null
        };

        return await todoItemRepository.TodoItemUpdateAsync(updated, cancellationToken);
    }

    private DateTimeOffset? DetermineCompletedAt(bool wasCompleted, bool isNowCompleted, DateTimeOffset? existingCompletedAt)
    {
        if (!wasCompleted && isNowCompleted)
            return timeProvider.GetUtcNow();

        if (wasCompleted && !isNowCompleted)
            return null;

        return existingCompletedAt;
    }

    private static void ValidateOwnership(TodoItem todo, int userId)
    {
        if (todo.UserId != userId)
            throw new UnauthorizedAccessException("You do not have permission to modify this todo.");
    }
}
