using System.ComponentModel.DataAnnotations;

namespace TodoApp.Interfaces.DTOs.Tasks;

public class CreateTaskRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public DateOnly? DueDate { get; set; }

    public int? CategoryId { get; set; }
}
