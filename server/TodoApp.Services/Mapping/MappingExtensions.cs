using TodoApp.Interfaces.DTOs.Categories;
using TodoApp.Interfaces.DTOs.Tasks;
using TodoApp.Interfaces.Entities;

namespace TodoApp.Services.Mapping;

/// <summary>
/// Manual entity -> DTO mapping. Simple and explicit, no AutoMapper needed.
/// </summary>
public static class MappingExtensions
{
    public static CategoryDto ToDto(this Category category) => new()
    {
        Id = category.Id,
        Name = category.Name
    };

    public static TaskDto ToDto(this TodoItem task) => new()
    {
        Id = task.Id,
        Title = task.Title,
        Description = task.Description,
        IsCompleted = task.IsCompleted,
        DueDate = task.DueDate,
        CreatedAt = task.CreatedAt,
        CategoryId = task.CategoryId,
        CategoryName = task.Category?.Name
    };
}
