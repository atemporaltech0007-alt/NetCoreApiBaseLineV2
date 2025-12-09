using ApiGenerico.Domain.Entities;
using ApiGenerico.Domain.Models.Dto;

namespace ApiGenerico.Application.Services.Interfaces;

public interface IStateService
{
    Task<IEnumerable<StateDto>> GetAllAsync();
    Task<StateDto?> GetByIdAsync(int id);
    Task<StateDto> CreateAsync(CreateStateDto dto);
    Task<StateDto> UpdateAsync(int id, UpdateStateDto dto);
    Task DeleteAsync(int id);
}
