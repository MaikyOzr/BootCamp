namespace BootCamp.Application.Feature.TaskCommentFeature.Models.Request;

public record UpdateTaskCommentRequest
{
    public string? Content { get; set; }
}
