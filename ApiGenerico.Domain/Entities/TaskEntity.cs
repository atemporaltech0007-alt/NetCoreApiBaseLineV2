namespace ApiGenerico.Domain.Entities;

public class TaskEntity
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public int StateId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public byte[] RowVersion { get; set; }

    public State State { get; set; }
    public ICollection<TaskHistory> TaskHistories { get; set; }
}
