using ApiGenerico.Domain.Entities;
using ApiGenerico.Domain.Events;
using ApiGenerico.Infrastructure.Context;
using MediatR;

namespace ApiGenerico.Application.Handlers;

public class TaskStateChangedEventHandler : INotificationHandler<TaskStateChangedEvent>
{
    private readonly ContextSql _context;

    public TaskStateChangedEventHandler(ContextSql context)
    {
        _context = context;
    }

    public async Task Handle(TaskStateChangedEvent notification, CancellationToken cancellationToken)
    {
        var history = new TaskHistory
        {
            TaskId = notification.TaskId,
            PreviousStateId = notification.PreviousStateId,
            NewStateId = notification.NewStateId,
            ChangedAt = notification.ChangedAt
        };

        _context.TaskHistories.Add(history);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
