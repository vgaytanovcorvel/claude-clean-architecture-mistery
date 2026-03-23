using MisteryApp.Abstractions.Models;
using MisteryApp.Abstractions.Models.Requests;

namespace MisteryApp.Abstractions.Interfaces;

public interface ITodoService
{
    Task<IReadOnlyList<TodoItem>> GetTodosByUserIdAsync(int userId, CancellationToken cancellationToken);
    Task<TodoItem> CreateTodoAsync(int userId, CreateTodoRequest request, CancellationToken cancellationToken);
    Task<TodoItem> UpdateTodoAsync(int todoItemId, int userId, UpdateTodoRequest request, CancellationToken cancellationToken);
    Task DeleteTodoAsync(int todoItemId, int userId, CancellationToken cancellationToken);
    Task<TodoItem> ToggleTodoAsync(int todoItemId, int userId, CancellationToken cancellationToken);
}
