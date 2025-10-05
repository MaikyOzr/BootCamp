namespace BootCamp.Application.Feature.TaskCommentFeature.Models.Response;

public record GetByIdTaskCommentResponse
{
    public string? Content { get; set; }
    public Guid TaskId { get; set; }
}
