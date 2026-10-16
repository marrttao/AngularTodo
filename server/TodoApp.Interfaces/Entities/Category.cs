namespace TodoApp.Interfaces.Entities;

/// <summary>
/// A user-defined list of tasks (like "Work" or "Groceries" in Microsoft To-Do).
/// </summary>
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Owner of the category (foreign key + navigation property).
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public ICollection<TodoItem> Tasks { get; set; } = new List<TodoItem>();
}
