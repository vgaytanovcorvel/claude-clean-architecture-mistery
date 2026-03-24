using MisteryApp.Abstractions.Interfaces;
using MisteryApp.Abstractions.Models;

namespace MisteryApp.Implementation.Services;

public class TodoService(
    ITodoRepository todoRepository,
    IUserRepository userRepository,
    TimeProvider timeProvider) : ITodoService
{
    public virtual async Task<IReadOnlyList<TodoItem>> GetTodosByUserAsync(int userId, CancellationToken cancellationToken)
    {
        return await todoRepository.TodoFindByUserIdAsync(userId, cancellationToken);
    }

    public virtual async Task<TodoItem> GetTodoByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await todoRepository.TodoSingleByIdAsync(id, cancellationToken);
    }

    public virtual async Task<TodoItem> CreateTodoAsync(CreateTodoRequest request, CancellationToken cancellationToken)
    {
        await userRepository.UserSingleByIdAsync(request.UserId, cancellationToken);

        var todoItem = new TodoItem
        {
            Title = request.Title,
            UserId = request.UserId,
            CreatedAtUtc = timeProvider.GetUtcNow().UtcDateTime,
        };

        return await todoRepository.TodoAddAsync(todoItem, cancellationToken);
    }

    public virtual async Task<TodoItem> UpdateTodoAsync(int id, UpdateTodoRequest request, CancellationToken cancellationToken)
    {
        var existing = await todoRepository.TodoSingleByIdAsync(id, cancellationToken);

        var updated = new TodoItem
        {
            Id = existing.Id,
            Title = request.Title,
            IsComplete = existing.IsComplete,
            UserId = existing.UserId,
            CreatedAtUtc = existing.CreatedAtUtc,
            CompletedAtUtc = existing.CompletedAtUtc,
        };

        return await todoRepository.TodoUpdateAsync(updated, cancellationToken);
    }

    public virtual async Task<TodoItem> ToggleTodoCompleteAsync(int id, CancellationToken cancellationToken)
    {
        var existing = await todoRepository.TodoSingleByIdAsync(id, cancellationToken);
        var now = timeProvider.GetUtcNow().UtcDateTime;

        var toggled = new TodoItem
        {
            Id = existing.Id,
            Title = existing.Title,
            IsComplete = !existing.IsComplete,
            UserId = existing.UserId,
            CreatedAtUtc = existing.CreatedAtUtc,
            CompletedAtUtc = !existing.IsComplete ? now : null,
        };

        return await todoRepository.TodoUpdateAsync(toggled, cancellationToken);
    }

    public virtual async Task DeleteTodoAsync(int id, CancellationToken cancellationToken)
    {
        await todoRepository.TodoDeleteAsync(id, cancellationToken);
    }
}
