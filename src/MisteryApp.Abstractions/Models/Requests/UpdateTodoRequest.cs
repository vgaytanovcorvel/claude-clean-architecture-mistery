namespace MisteryApp.Abstractions.Models.Requests;

public record UpdateTodoRequest(string Title, string? Description, bool IsCompleted);
