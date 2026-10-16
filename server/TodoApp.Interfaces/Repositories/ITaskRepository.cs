using TodoApp.Interfaces.DTOs.Tasks;
using TodoApp.Interfaces.Entities;

namespace TodoApp.Interfaces.Repositories;

public interface ITaskRepository
{
    /// <summary>
    /// Returns one page of the user's tasks after applying search and filters,
    /// plus the total number of matching tasks (needed to calculate the number of pages).
    /// </summary>
    Task<(IReadOnlyList<TodoItem> Items, int TotalCount)> GetPagedAsync(int userId, TaskQueryParameters query);

    Task<TodoItem?> GetByIdAsync(int id, int userId);
    Task AddAsync(TodoItem task);
    Task UpdateAsync(TodoItem task);
    Task DeleteAsync(TodoItem task);
}
