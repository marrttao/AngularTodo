namespace TodoApp.Interfaces.Entities;

/// <summary>
/// A single task. Named TodoItem (not Task) to avoid a conflict with System.Threading.Tasks.Task.
/// </summary>
public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }

    // DateOnly: a due date is a calendar day, so there is no time zone to worry about.
    public DateOnly? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }

    // Optional category: a task can exist without a category.
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    // Owner of the task.
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
