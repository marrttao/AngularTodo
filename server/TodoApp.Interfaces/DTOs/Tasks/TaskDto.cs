namespace TodoApp.Interfaces.DTOs.Tasks;

public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateOnly? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? CategoryId { get; set; }

    // Denormalized for the UI, so the client doesn't need a second request to show the category name.
    public string? CategoryName { get; set; }
}
