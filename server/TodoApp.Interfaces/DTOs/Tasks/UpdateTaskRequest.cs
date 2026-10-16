using System.ComponentModel.DataAnnotations;

namespace TodoApp.Interfaces.DTOs.Tasks;

/// <summary>
/// Full update (PUT): the client sends all editable fields of the task.
/// </summary>
public class UpdateTaskRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public bool IsCompleted { get; set; }

    public DateOnly? DueDate { get; set; }

    public int? CategoryId { get; set; }
}
