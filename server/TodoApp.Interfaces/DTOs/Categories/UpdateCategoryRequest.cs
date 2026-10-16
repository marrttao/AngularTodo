using System.ComponentModel.DataAnnotations;

namespace TodoApp.Interfaces.DTOs.Categories;

public class UpdateCategoryRequest
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}
