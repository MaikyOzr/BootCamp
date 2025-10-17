namespace BootCamp.TaskService.Application.TaskFeature.Models.Request;

public record UpdateTaskRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
}
