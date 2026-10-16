namespace TodoApp.Interfaces.Entities;

/// <summary>
/// Application user. We store only a BCrypt hash of the password, never the password itself.
/// </summary>
public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    // Navigation properties: one user has many categories and many tasks.
    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<TodoItem> Tasks { get; set; } = new List<TodoItem>();
}
