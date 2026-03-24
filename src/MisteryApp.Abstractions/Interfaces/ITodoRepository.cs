using MisteryApp.Abstractions.Models;

namespace MisteryApp.Abstractions.Interfaces;

public interface ITodoRepository
{
    Task<IReadOnlyList<TodoItem>> TodoFindByUserIdAsync(int userId, CancellationToken cancellationToken);
    Task<TodoItem> TodoSingleByIdAsync(int id, CancellationToken cancellationToken);
    Task<TodoItem> TodoAddAsync(TodoItem todoItem, CancellationToken cancellationToken);
    Task<TodoItem> TodoUpdateAsync(TodoItem todoItem, CancellationToken cancellationToken);
    Task TodoDeleteAsync(int id, CancellationToken cancellationToken);
}
