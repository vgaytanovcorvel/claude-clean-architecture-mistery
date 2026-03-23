namespace MisteryApp.Repository.Entities;

public class TodoItemEntity
{
    public int TodoItemId { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public UserEntity User { get; set; } = null!;
}
