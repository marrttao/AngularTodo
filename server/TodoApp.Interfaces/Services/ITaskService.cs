using TodoApp.Interfaces.DTOs.Common;
using TodoApp.Interfaces.DTOs.Tasks;

namespace TodoApp.Interfaces.Services;

public interface ITaskService
{
    Task<PagedResult<TaskDto>> GetPagedAsync(int userId, TaskQueryParameters query);
    Task<TaskDto> GetByIdAsync(int id, int userId);
    Task<TaskDto> CreateAsync(int userId, CreateTaskRequest request);
    Task<TaskDto> UpdateAsync(int id, int userId, UpdateTaskRequest request);
    Task DeleteAsync(int id, int userId);
}
