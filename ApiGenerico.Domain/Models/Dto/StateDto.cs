namespace ApiGenerico.Domain.Models.Dto;

public class StateDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateStateDto
{
    public string Name { get; set; }
}

public class UpdateStateDto
{
    public string Name { get; set; }
}
