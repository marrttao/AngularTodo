using Microsoft.EntityFrameworkCore;
using TodoApp.Interfaces.DTOs.Tasks;
using TodoApp.Interfaces.Entities;
using TodoApp.Interfaces.Repositories;

namespace TodoApp.DataAccess.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _db;

    public TaskRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(IReadOnlyList<TodoItem> Items, int TotalCount)> GetPagedAsync(
        int userId, TaskQueryParameters query)
    {
        // IQueryable: nothing is sent to the database yet, we only build the SQL query step by step.
        IQueryable<TodoItem> tasks = _db.Tasks
            .AsNoTracking()
            .Include(t => t.Category) // JOIN Categories to get the category name
            .Where(t => t.UserId == userId);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            // Translated to SQL "LIKE '%search%'" (case-insensitive with the default SQL Server collation).
            tasks = tasks.Where(t =>
                t.Title.Contains(search) ||
                (t.Description != null && t.Description.Contains(search)));
        }

        if (query.CategoryId.HasValue)
        {
            tasks = tasks.Where(t => t.CategoryId == query.CategoryId.Value);
        }

        if (query.IsCompleted.HasValue)
        {
            tasks = tasks.Where(t => t.IsCompleted == query.IsCompleted.Value);
        }

        // First query: SELECT COUNT(*) with the same filters.
        var totalCount = await tasks.CountAsync();

        // Second query: only one page of rows (OFFSET ... FETCH NEXT ... in SQL).
        // Active tasks first, then newest first; Id makes the order stable between pages.
        var items = await tasks
            .OrderBy(t => t.IsCompleted)
            .ThenByDescending(t => t.CreatedAt)
            .ThenByDescending(t => t.Id)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public Task<TodoItem?> GetByIdAsync(int id, int userId) =>
        _db.Tasks
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

    public async Task AddAsync(TodoItem task)
    {
        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(TodoItem task)
    {
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(TodoItem task)
    {
        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();
    }
}
