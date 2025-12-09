namespace ApiGenerico.Domain.Entities;

public class State
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<TaskEntity> Tasks { get; set; }
}
