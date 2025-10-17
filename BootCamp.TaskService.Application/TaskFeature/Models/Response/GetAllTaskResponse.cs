using BootCamp.TaskService.Domain.Entity;

namespace BootCamp.TaskService.Application.TaskFeature.Models.Response;

public record GetAllTaskResponse
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public Guid UserId { get; set; }
    public List<TaskComment>? Comments { get; set; }
    public uint? RowVersion { get; set; }
}
