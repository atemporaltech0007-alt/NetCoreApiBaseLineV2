using ApiGenerico.Domain.Entities;
using ApiGenerico.Domain.Models.Dto;
using ApiGenerico.Infrastructure.Context;
using ApiGenerico.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApiGenerico.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly ContextSql _context;

    public TaskRepository(ContextSql context)
    {
        _context = context;
    }

    public async Task<PagedResultDto<TaskEntity>> GetAllAsync(int page, int pageSize, TaskFilterDto filter)
    {
        var query = _context.Tasks
            .Include(t => t.State)
            .AsQueryable();

        if (filter.StateId.HasValue)
        {
            query = query.Where(t => t.StateId == filter.StateId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Title))
        {
            query = query.Where(t => t.Title.Contains(filter.Title));
        }

        if (filter.DueDateFrom.HasValue)
        {
            query = query.Where(t => t.DueDate >= filter.DueDateFrom.Value);
        }

        if (filter.DueDateTo.HasValue)
        {
            query = query.Where(t => t.DueDate <= filter.DueDateTo.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(t => t.DueDate)
            .ThenBy(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResultDto<TaskEntity>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<TaskEntity?> GetByIdAsync(int id)
    {
        return await _context.Tasks
            .Include(t => t.State)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<TaskEntity> CreateAsync(TaskEntity task)
    {
        task.CreatedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;
        
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        
        return task;
    }

    public async Task<TaskEntity> UpdateAsync(TaskEntity task)
    {
        task.UpdatedAt = DateTime.UtcNow;
        
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
        
        return task;
    }

    public async Task DeleteAsync(int id)
    {
        var task = await GetByIdAsync(id);
        if (task != null)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<State>> GetStatesAsync()
    {
        return await _context.States
            .OrderBy(s => s.Name)
            .ToListAsync();
    }
}
