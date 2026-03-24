namespace MisteryApp.Repository.Entities;

public class TodoItemEntity
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsComplete { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public UserEntity User { get; set; } = null!;
}
