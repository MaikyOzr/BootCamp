namespace BootCamp.Application.Feature.TaskCommentFeature.Models.Response;

public record GetAllTaskCommentResponse
{
    public string? Content { get; set; }
    public Guid TaskId { get; set; }
    public uint RowVersion { get; set; }
}
