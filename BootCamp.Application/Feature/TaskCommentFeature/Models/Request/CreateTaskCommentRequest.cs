namespace BootCamp.Application.Feature.TaskCommentFeature.Models.Request;

public record CreateTaskCommentRequest
{
    public string? Content { get; set; }
    public Guid TaskId { get; set; }
}
