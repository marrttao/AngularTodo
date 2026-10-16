using TodoApp.Interfaces.DTOs.Categories;

namespace TodoApp.Interfaces.Services;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDto>> GetAllAsync(int userId);
    Task<CategoryDto> GetByIdAsync(int id, int userId);
    Task<CategoryDto> CreateAsync(int userId, CreateCategoryRequest request);
    Task<CategoryDto> UpdateAsync(int id, int userId, UpdateCategoryRequest request);
    Task DeleteAsync(int id, int userId);
}
