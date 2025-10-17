namespace BootCamp.TaskService.Application.TaskFeature.Models.Request;

public record CreateTaskRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public Guid UserId { get; set; }
}
