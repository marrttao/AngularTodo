using System.ComponentModel.DataAnnotations;

namespace TodoApp.Interfaces.DTOs.Categories;

public class CreateCategoryRequest
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}
