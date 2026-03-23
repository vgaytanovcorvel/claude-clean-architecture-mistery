namespace MisteryApp.Repository.Entities;

public class UserEntity
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public ICollection<TodoItemEntity> TodoItems { get; set; } = [];
}
