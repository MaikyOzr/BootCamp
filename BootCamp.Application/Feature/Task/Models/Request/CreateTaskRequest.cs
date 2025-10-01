namespace BootCamp.Application.Feature.Task.Models.Request;

public record CreateTaskRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public Guid UserId { get; set; }
}
