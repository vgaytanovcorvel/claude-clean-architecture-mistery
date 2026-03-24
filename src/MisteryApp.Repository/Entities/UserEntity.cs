namespace MisteryApp.Repository.Entities;

public class UserEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public ICollection<TodoItemEntity> Todos { get; set; } = [];
}
