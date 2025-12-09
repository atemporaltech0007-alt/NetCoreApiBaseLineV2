using ApiGenerico.Domain.Entities;
using ApiGenerico.Domain.Models.Dto;

namespace ApiGenerico.Infrastructure.Repositories.Interfaces;

public interface IStateRepository
{
    Task<IEnumerable<State>> GetAllAsync();
    Task<State?> GetByIdAsync(int id);
    Task<State?> GetByNameAsync(string name);
    Task<State> CreateAsync(State state);
    Task<State> UpdateAsync(State state);
    Task DeleteAsync(int id);
    Task<bool> HasTasksAsync(int stateId);
}
