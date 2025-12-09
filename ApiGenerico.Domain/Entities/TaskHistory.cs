namespace ApiGenerico.Domain.Entities;

public class TaskHistory
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public int? PreviousStateId { get; set; }
    public int NewStateId { get; set; }
    public DateTime ChangedAt { get; set; }

    public TaskEntity Task { get; set; }
    public State PreviousState { get; set; }
    public State NewState { get; set; }
}
