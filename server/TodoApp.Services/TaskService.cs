using TodoApp.Interfaces.DTOs.Common;
using TodoApp.Interfaces.DTOs.Tasks;
using TodoApp.Interfaces.Entities;
using TodoApp.Interfaces.Exceptions;
using TodoApp.Interfaces.Repositories;
using TodoApp.Interfaces.Services;
using TodoApp.Services.Mapping;

namespace TodoApp.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly ICategoryRepository _categoryRepository;

    public TaskService(ITaskRepository taskRepository, ICategoryRepository categoryRepository)
    {
        _taskRepository = taskRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<PagedResult<TaskDto>> GetPagedAsync(int userId, TaskQueryParameters query)
    {
        var (items, totalCount) = await _taskRepository.GetPagedAsync(userId, query);

        return new PagedResult<TaskDto>
        {
            Items = items.Select(t => t.ToDto()).ToList(),
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize,
            // e.g. 21 tasks / 10 per page = 2.1 -> 3 pages
            TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
        };
    }

    public async Task<TaskDto> GetByIdAsync(int id, int userId)
    {
        var task = await GetOwnedTaskAsync(id, userId);
        return task.ToDto();
    }

    public async Task<TaskDto> CreateAsync(int userId, CreateTaskRequest request)
    {
        var category = await GetCategoryOrNullAsync(request.CategoryId, userId);

        var task = new TodoItem
        {
            Title = request.Title.Trim(),
            Description = NormalizeDescription(request.Description),
            DueDate = request.DueDate,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow,
            UserId = userId,
            CategoryId = category?.Id,
            Category = category // already tracked by EF Core, so it won't be inserted again
        };

        await _taskRepository.AddAsync(task);
        return task.ToDto();
    }

    public async Task<TaskDto> UpdateAsync(int id, int userId, UpdateTaskRequest request)
    {
        var task = await GetOwnedTaskAsync(id, userId);
        var category = await GetCategoryOrNullAsync(request.CategoryId, userId);

        task.Title = request.Title.Trim();
        task.Description = NormalizeDescription(request.Description);
        task.IsCompleted = request.IsCompleted;
        task.DueDate = request.DueDate;
        task.CategoryId = category?.Id;
        task.Category = category;

        await _taskRepository.UpdateAsync(task);
        return task.ToDto();
    }

    public async Task DeleteAsync(int id, int userId)
    {
        var task = await GetOwnedTaskAsync(id, userId);
        await _taskRepository.DeleteAsync(task);
    }

    private async Task<TodoItem> GetOwnedTaskAsync(int id, int userId)
    {
        return await _taskRepository.GetByIdAsync(id, userId)
            ?? throw new NotFoundException($"Task with id {id} was not found.");
    }

    /// <summary>
    /// A task may reference only a category of the same user.
    /// </summary>
    private async Task<Category?> GetCategoryOrNullAsync(int? categoryId, int userId)
    {
        if (categoryId is null)
        {
            return null;
        }

        return await _categoryRepository.GetByIdAsync(categoryId.Value, userId)
            ?? throw new BadRequestException($"Category with id {categoryId} does not exist.");
    }

    private static string? NormalizeDescription(string? description) =>
        string.IsNullOrWhiteSpace(description) ? null : description.Trim();
}
