using ApiGenerico.Domain.Entities;
using ApiGenerico.Domain.Models.Dto;

namespace ApiGenerico.Infrastructure.Repositories.Interfaces;

public interface ITaskRepository
{
    Task<PagedResultDto<TaskEntity>> GetAllAsync(int page, int pageSize, TaskFilterDto filter);
    Task<TaskEntity?> GetByIdAsync(int id);
    Task<TaskEntity> CreateAsync(TaskEntity task);
    Task<TaskEntity> UpdateAsync(TaskEntity task);
    Task DeleteAsync(int id);
    Task<IEnumerable<State>> GetStatesAsync();
}
