using ApiGenerico.Domain.Models.Dto;

namespace ApiGenerico.Application.Services.Interfaces;

public interface ITaskService
{
    Task<PagedResultDto<TaskDto>> GetAllAsync(int page, int pageSize, TaskFilterDto filter);
    Task<TaskDto?> GetByIdAsync(int id);
    Task<TaskDto> CreateAsync(CreateTaskDto dto);
    Task<TaskDto> UpdateAsync(int id, UpdateTaskDto dto);
    Task DeleteAsync(int id);
    Task<IEnumerable<StateDto>> GetStatesAsync();
}
