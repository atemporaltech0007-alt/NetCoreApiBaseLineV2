using ApiGenerico.Application.Services.Interfaces;
using ApiGenerico.Domain.Entities;
using ApiGenerico.Domain.Models.Dto;
using ApiGenerico.Infrastructure.Repositories.Interfaces;

namespace ApiGenerico.Application.Services;

public class StateService : IStateService
{
    private readonly IStateRepository _stateRepository;

    public StateService(IStateRepository stateRepository)
    {
        _stateRepository = stateRepository;
    }

    public async Task<IEnumerable<StateDto>> GetAllAsync()
    {
        var states = await _stateRepository.GetAllAsync();
        return states.Select(MapToDto);
    }

    public async Task<StateDto?> GetByIdAsync(int id)
    {
        var state = await _stateRepository.GetByIdAsync(id);
        return state != null ? MapToDto(state) : null;
    }

    public async Task<StateDto> CreateAsync(CreateStateDto dto)
    {
        var existingState = await _stateRepository.GetByNameAsync(dto.Name);
        if (existingState != null)
        {
            throw new InvalidOperationException($"State with name '{dto.Name}' already exists.");
        }

        var state = new State
        {
            Name = dto.Name
        };

        var created = await _stateRepository.CreateAsync(state);
        return MapToDto(created);
    }

    public async Task<StateDto> UpdateAsync(int id, UpdateStateDto dto)
    {
        var state = await _stateRepository.GetByIdAsync(id);
        if (state == null)
        {
            throw new KeyNotFoundException($"State with id {id} not found.");
        }

        var existingState = await _stateRepository.GetByNameAsync(dto.Name);
        if (existingState != null && existingState.Id != id)
        {
            throw new InvalidOperationException($"State with name '{dto.Name}' already exists.");
        }

        state.Name = dto.Name;
        var updated = await _stateRepository.UpdateAsync(state);
        return MapToDto(updated);
    }

    public async Task DeleteAsync(int id)
    {
        var state = await _stateRepository.GetByIdAsync(id);
        if (state == null)
        {
            throw new KeyNotFoundException($"State with id {id} not found.");
        }

        var hasTasks = await _stateRepository.HasTasksAsync(id);
        if (hasTasks)
        {
            throw new InvalidOperationException($"Cannot delete state '{state.Name}' because it has associated tasks.");
        }

        await _stateRepository.DeleteAsync(id);
    }

    private StateDto MapToDto(State state)
    {
        return new StateDto
        {
            Id = state.Id,
            Name = state.Name,
            CreatedAt = state.CreatedAt,
            UpdatedAt = state.UpdatedAt
        };
    }
}
