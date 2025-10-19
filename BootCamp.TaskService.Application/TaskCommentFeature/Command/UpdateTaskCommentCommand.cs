using BootCamp.Contract.Events;
using BootCamp.TaskService.Application.Common;
using BootCamp.TaskService.Application.TaskCommentFeature.Models.Request;
using BootCamp.TaskService.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BootCamp.TaskService.Application.TaskCommentFeature.Command;

public class UpdateTaskCommentCommand(TaskServiceDbContext context, IOutboxWriter outboxWriter,
     IHttpContextAccessor accessor)
{
    public async Task<BaseResponse> ExecuteAsync(Guid id, UpdateTaskCommentRequest request, CancellationToken ct)
    {
        var taskComment = await context.TaskComments.Where(x => x.Id == id)
             .SingleOrDefaultAsync(ct);

        taskComment.Content = request.Content;

        await context.SaveChangesAsync(ct);

        var correlationId = accessor.HttpContext?.Request.Headers["X-Correlation-Id"].FirstOrDefault()
                            ?? Guid.NewGuid().ToString();

        var taskCommentUpdated = new TaskCommentUpdatedV1(
        TaskId: taskComment.Id,
        Content: taskComment.Content,
        CreateAt: DateTime.UtcNow,
        CorrelationId: correlationId
        );

        return new() { Id = taskComment.Id };
    }
}
