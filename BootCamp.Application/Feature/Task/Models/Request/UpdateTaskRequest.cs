namespace BootCamp.Application.Feature.Task.Models.Request;

public record UpdateTaskRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
}
