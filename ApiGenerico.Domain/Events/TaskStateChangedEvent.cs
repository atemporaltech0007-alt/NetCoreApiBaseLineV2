using MediatR;

namespace ApiGenerico.Domain.Events;

public class TaskStateChangedEvent : INotification
{
    public int TaskId { get; set; }
    public int? PreviousStateId { get; set; }
    public int NewStateId { get; set; }
    public DateTime ChangedAt { get; set; }

    public TaskStateChangedEvent(int taskId, int? previousStateId, int newStateId)
    {
        TaskId = taskId;
        PreviousStateId = previousStateId;
        NewStateId = newStateId;
        ChangedAt = DateTime.UtcNow;
    }
}
