using MisteryApp.Abstractions.Models;

namespace MisteryApp.Abstractions.Interfaces;

public interface ITodoService
{
    Task<IReadOnlyList<TodoItem>> GetTodosByUserAsync(int userId, CancellationToken cancellationToken);
    Task<TodoItem> GetTodoByIdAsync(int id, CancellationToken cancellationToken);
    Task<TodoItem> CreateTodoAsync(CreateTodoRequest request, CancellationToken cancellationToken);
    Task<TodoItem> UpdateTodoAsync(int id, UpdateTodoRequest request, CancellationToken cancellationToken);
    Task<TodoItem> ToggleTodoCompleteAsync(int id, CancellationToken cancellationToken);
    Task DeleteTodoAsync(int id, CancellationToken cancellationToken);
}
