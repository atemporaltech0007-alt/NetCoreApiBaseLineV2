using ApiGenerico.Application.Services.Interfaces;
using ApiGenerico.Domain.Models.Dto;
using Microsoft.Extensions.Caching.Memory;

namespace ApiGenerico.Application.Services.Decorators;

public class CachedStateService : IStateService
{
    private readonly IStateService _innerService;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "States_All";
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

    public CachedStateService(IStateService innerService, IMemoryCache cache)
    {
        _innerService = innerService;
        _cache = cache;
    }

    public async Task<IEnumerable<StateDto>> GetAllAsync()
    {
        if (_cache.TryGetValue(CacheKey, out IEnumerable<StateDto>? cachedStates) && cachedStates != null)
        {
            return cachedStates;
        }

        var states = await _innerService.GetAllAsync();
        _cache.Set(CacheKey, states, _cacheDuration);
        return states;
    }

    public Task<StateDto?> GetByIdAsync(int id)
    {
        return _innerService.GetByIdAsync(id);
    }

    public async Task<StateDto> CreateAsync(CreateStateDto dto)
    {
        var state = await _innerService.CreateAsync(dto);
        InvalidateCache();
        return state;
    }

    public async Task<StateDto> UpdateAsync(int id, UpdateStateDto dto)
    {
        var state = await _innerService.UpdateAsync(id, dto);
        InvalidateCache();
        return state;
    }

    public async Task DeleteAsync(int id)
    {
        await _innerService.DeleteAsync(id);
        InvalidateCache();
    }

    private void InvalidateCache()
    {
        _cache.Remove(CacheKey);
    }
}
