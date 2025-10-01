using BootCamp.Domain;

namespace BootCamp.Application.Feature.Task.Models.Response;

public class GetAllTaskResponse
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public Guid UserId { get; set; }
    public List<TaskComment>? Comments { get; set; }
}
