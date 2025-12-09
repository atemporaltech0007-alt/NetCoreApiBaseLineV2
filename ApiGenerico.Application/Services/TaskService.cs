using ApiGenerico.Application.Services.Interfaces;
using ApiGenerico.Domain.Entities;
using ApiGenerico.Domain.Events;
using ApiGenerico.Domain.Models.Dto;
using ApiGenerico.Infrastructure.Repositories.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApiGenerico.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IStateRepository _stateRepository;
    private readonly IMediator _mediator;

    public TaskService(ITaskRepository taskRepository, IStateRepository stateRepository, IMediator mediator)
    {
        _taskRepository = taskRepository;
        _stateRepository = stateRepository;
        _mediator = mediator;
    }

    public async Task<PagedResultDto<TaskDto>> GetAllAsync(int page, int pageSize, TaskFilterDto filter)
    {
        var pagedResult = await _taskRepository.GetAllAsync(page, pageSize, filter);

        return new PagedResultDto<TaskDto>
        {
            Items = pagedResult.Items.Select(MapToDto).ToList(),
            TotalCount = pagedResult.TotalCount,
            Page = pagedResult.Page,
            PageSize = pagedResult.PageSize
        };
    }

    public async Task<TaskDto?> GetByIdAsync(int id)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        return task != null ? MapToDto(task) : null;
    }

    public async Task<TaskDto> CreateAsync(CreateTaskDto dto)
    {
        var state = await _stateRepository.GetByIdAsync(dto.StateId);
        if (state == null)
        {
            throw new KeyNotFoundException($"State with id {dto.StateId} not found.");
        }

        var task = new TaskEntity
        {
            Title = dto.Title,
            Description = dto.Description,
            DueDate = dto.DueDate,
            StateId = dto.StateId
        };

        var created = await _taskRepository.CreateAsync(task);
        var fullTask = await _taskRepository.GetByIdAsync(created.Id);
        return MapToDto(fullTask!);
    }

    public async Task<TaskDto> UpdateAsync(int id, UpdateTaskDto dto)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null)
        {
            throw new KeyNotFoundException($"Task with id {id} not found.");
        }

        if (!task.RowVersion.SequenceEqual(dto.RowVersion))
        {
            throw new DbUpdateConcurrencyException("The task has been modified by another user. Please refresh and try again.");
        }

        var state = await _stateRepository.GetByIdAsync(dto.StateId);
        if (state == null)
        {
            throw new KeyNotFoundException($"State with id {dto.StateId} not found.");
        }

        var previousStateId = task.StateId;
        var stateChanged = previousStateId != dto.StateId;

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.DueDate = dto.DueDate;
        task.StateId = dto.StateId;

        var updated = await _taskRepository.UpdateAsync(task);

        if (stateChanged)
        {
            await _mediator.Publish(new TaskStateChangedEvent(updated.Id, previousStateId, dto.StateId));
        }

        var fullTask = await _taskRepository.GetByIdAsync(updated.Id);
        return MapToDto(fullTask!);
    }

    public async Task DeleteAsync(int id)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null)
        {
            throw new KeyNotFoundException($"Task with id {id} not found.");
        }

        await _taskRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<StateDto>> GetStatesAsync()
    {
        var states = await _taskRepository.GetStatesAsync();
        return states.Select(s => new StateDto
        {
            Id = s.Id,
            Name = s.Name,
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt
        });
    }

    private TaskDto MapToDto(TaskEntity task)
    {
        return new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            StateId = task.StateId,
            StateName = task.State?.Name ?? string.Empty,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt,
            RowVersion = task.RowVersion
        };
    }
}
