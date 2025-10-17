namespace BootCamp.TaskService.Application.TaskCommentFeature.Models.Request;

public record CreateTaskCommentRequest(string? Content, Guid TaskId);
