using TodoApp.Interfaces.Entities;

namespace TodoApp.Interfaces.Repositories;

/// <summary>
/// Every method takes userId, so a user can never read or change another user's categories.
/// </summary>
public interface ICategoryRepository
{
    Task<IReadOnlyList<Category>> GetAllAsync(int userId);
    Task<Category?> GetByIdAsync(int id, int userId);

    /// <param name="excludeId">Category to ignore (used when renaming a category to its own name).</param>
    Task<bool> ExistsByNameAsync(int userId, string name, int? excludeId = null);

    Task AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(Category category);
}
