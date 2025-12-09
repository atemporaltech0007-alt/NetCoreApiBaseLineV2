using ApiGenerico.Application.Services.Interfaces;
using ApiGenerico.Domain.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiGenerico.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StatesController : ControllerBase
{
    private readonly IStateService _stateService;

    public StatesController(IStateService stateService)
    {
        _stateService = stateService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StateDto>>> GetAll()
    {
        var states = await _stateService.GetAllAsync();
        return Ok(states);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StateDto>> GetById(int id)
    {
        var state = await _stateService.GetByIdAsync(id);
        if (state == null)
        {
            return NotFound(new { message = $"State with id {id} not found." });
        }

        return Ok(state);
    }

    [HttpPost]
    public async Task<ActionResult<StateDto>> Create([FromBody] CreateStateDto dto)
    {
        var state = await _stateService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = state.Id }, state);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<StateDto>> Update(int id, [FromBody] UpdateStateDto dto)
    {
        var state = await _stateService.UpdateAsync(id, dto);
        return Ok(state);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _stateService.DeleteAsync(id);
        return NoContent();
    }
}
