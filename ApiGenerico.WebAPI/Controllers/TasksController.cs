using ApiGenerico.Application.Services.Interfaces;
using ApiGenerico.Domain.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiGenerico.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<TaskDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? stateId = null,
        [FromQuery] string? title = null,
        [FromQuery] DateTime? dueDateFrom = null,
        [FromQuery] DateTime? dueDateTo = null)
    {
        var filter = new TaskFilterDto
        {
            StateId = stateId,
            Title = title,
            DueDateFrom = dueDateFrom,
            DueDateTo = dueDateTo
        };

        var result = await _taskService.GetAllAsync(page, pageSize, filter);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskDto>> GetById(int id)
    {
        var task = await _taskService.GetByIdAsync(id);
        if (task == null)
        {
            return NotFound(new { message = $"Task with id {id} not found." });
        }

        return Ok(task);
    }

    [HttpGet("states")]
    public async Task<ActionResult<IEnumerable<StateDto>>> GetStates()
    {
        var states = await _taskService.GetStatesAsync();
        return Ok(states);
    }

    [HttpPost]
    public async Task<ActionResult<TaskDto>> Create([FromBody] CreateTaskDto dto)
    {
        var task = await _taskService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TaskDto>> Update(int id, [FromBody] UpdateTaskDto dto)
    {
        var task = await _taskService.UpdateAsync(id, dto);
        return Ok(task);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _taskService.DeleteAsync(id);
        return NoContent();
    }
}
