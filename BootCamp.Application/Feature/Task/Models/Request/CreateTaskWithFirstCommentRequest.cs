namespace BootCamp.Application.Feature.Task.Models.Request;

public record CreateTaskWithFirstCommentRequest
{
    public string? TaskTitle { get; set; }
    public string? TaskDescription { get; set; }
    public string? TaskComment { get; set; }
    public Guid UserId { get; set; }
}
