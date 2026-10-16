using Microsoft.EntityFrameworkCore;
using TodoApp.Interfaces.Entities;
using TodoApp.Interfaces.Repositories;

namespace TodoApp.DataAccess.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _db;

    public CategoryRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Category>> GetAllAsync(int userId) =>
        await _db.Categories
            .AsNoTracking() // read-only query: EF Core doesn't need to track changes
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.Name)
            .ToListAsync();

    // Tracked query: the returned entity can be modified and saved with UpdateAsync.
    public Task<Category?> GetByIdAsync(int id, int userId) =>
        _db.Categories.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

    public Task<bool> ExistsByNameAsync(int userId, string name, int? excludeId = null) =>
        _db.Categories.AnyAsync(c =>
            c.UserId == userId &&
            c.Name == name &&
            (excludeId == null || c.Id != excludeId));

    public async Task AddAsync(Category category)
    {
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Category category)
    {
        // The entity is tracked, so EF Core already knows which properties changed.
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Category category)
    {
        // Load the category's tasks into the context. Because the relationship is configured
        // with DeleteBehavior.ClientSetNull, EF Core sets their CategoryId to null
        // and saves it together with the delete in one transaction.
        await _db.Tasks.Where(t => t.CategoryId == category.Id).LoadAsync();

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();
    }
}
