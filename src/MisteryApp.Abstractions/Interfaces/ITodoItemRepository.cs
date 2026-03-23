using MisteryApp.Abstractions.Models;

namespace MisteryApp.Abstractions.Interfaces;

public interface ITodoItemRepository
{
    Task<TodoItem> TodoItemSingleByIdAsync(int id, CancellationToken cancellationToken);
    Task<TodoItem?> TodoItemSingleOrDefaultByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<TodoItem>> TodoItemGetAllByUserIdAsync(int userId, CancellationToken cancellationToken);
    Task<TodoItem> TodoItemAddAsync(TodoItem item, CancellationToken cancellationToken);
    Task<TodoItem> TodoItemUpdateAsync(TodoItem item, CancellationToken cancellationToken);
    Task TodoItemDeleteAsync(int id, CancellationToken cancellationToken);
}
