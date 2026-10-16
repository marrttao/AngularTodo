using System.ComponentModel.DataAnnotations;

namespace TodoApp.Interfaces.DTOs.Tasks;

/// <summary>
/// Query string parameters of GET /api/tasks,
/// e.g. /api/tasks?page=2&amp;pageSize=10&amp;search=milk&amp;categoryId=3&amp;isCompleted=false
/// </summary>
public class TaskQueryParameters
{
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 50)]
    public int PageSize { get; set; } = 10;

    /// <summary>Searches in title and description.</summary>
    [MaxLength(100)]
    public string? Search { get; set; }

    public int? CategoryId { get; set; }

    /// <summary>null = all tasks, true = only completed, false = only active.</summary>
    public bool? IsCompleted { get; set; }
}
