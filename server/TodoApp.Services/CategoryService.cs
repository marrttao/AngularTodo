using TodoApp.Interfaces.DTOs.Categories;
using TodoApp.Interfaces.Entities;
using TodoApp.Interfaces.Exceptions;
using TodoApp.Interfaces.Repositories;
using TodoApp.Interfaces.Services;
using TodoApp.Services.Mapping;

namespace TodoApp.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(int userId)
    {
        var categories = await _categoryRepository.GetAllAsync(userId);
        return categories.Select(c => c.ToDto()).ToList();
    }

    public async Task<CategoryDto> GetByIdAsync(int id, int userId)
    {
        var category = await GetOwnedCategoryAsync(id, userId);
        return category.ToDto();
    }

    public async Task<CategoryDto> CreateAsync(int userId, CreateCategoryRequest request)
    {
        var name = request.Name.Trim();
        await EnsureNameIsUniqueAsync(userId, name);

        var category = new Category { Name = name, UserId = userId };
        await _categoryRepository.AddAsync(category);

        return category.ToDto();
    }

    public async Task<CategoryDto> UpdateAsync(int id, int userId, UpdateCategoryRequest request)
    {
        var category = await GetOwnedCategoryAsync(id, userId);

        var name = request.Name.Trim();
        await EnsureNameIsUniqueAsync(userId, name, excludeId: id);

        category.Name = name;
        await _categoryRepository.UpdateAsync(category);

        return category.ToDto();
    }

    public async Task DeleteAsync(int id, int userId)
    {
        var category = await GetOwnedCategoryAsync(id, userId);
        await _categoryRepository.DeleteAsync(category);
    }

    private async Task<Category> GetOwnedCategoryAsync(int id, int userId)
    {
        // The repository filters by userId, so another user's category is simply "not found".
        return await _categoryRepository.GetByIdAsync(id, userId)
            ?? throw new NotFoundException($"Category with id {id} was not found.");
    }

    private async Task EnsureNameIsUniqueAsync(int userId, string name, int? excludeId = null)
    {
        if (await _categoryRepository.ExistsByNameAsync(userId, name, excludeId))
        {
            throw new ConflictException($"Category '{name}' already exists.");
        }
    }
}
