using ApiGenerico.Domain.Entities;
using ApiGenerico.Infrastructure.Context;
using ApiGenerico.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApiGenerico.Infrastructure.Repositories;

public class StateRepository : IStateRepository
{
    private readonly ContextSql _context;

    public StateRepository(ContextSql context)
    {
        _context = context;
    }

    public async Task<IEnumerable<State>> GetAllAsync()
    {
        return await _context.States
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<State?> GetByIdAsync(int id)
    {
        return await _context.States
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<State?> GetByNameAsync(string name)
    {
        return await _context.States
            .FirstOrDefaultAsync(s => s.Name == name);
    }

    public async Task<State> CreateAsync(State state)
    {
        state.CreatedAt = DateTime.UtcNow;
        state.UpdatedAt = DateTime.UtcNow;
        
        _context.States.Add(state);
        await _context.SaveChangesAsync();
        
        return state;
    }

    public async Task<State> UpdateAsync(State state)
    {
        state.UpdatedAt = DateTime.UtcNow;
        
        _context.States.Update(state);
        await _context.SaveChangesAsync();
        
        return state;
    }

    public async Task DeleteAsync(int id)
    {
        var state = await GetByIdAsync(id);
        if (state != null)
        {
            _context.States.Remove(state);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> HasTasksAsync(int stateId)
    {
        return await _context.Tasks.AnyAsync(t => t.StateId == stateId);
    }
}
