using BootCamp.Contract.Events;
using BootCamp.TaskService.Application.Common;
using BootCamp.TaskService.Application.TaskCommentFeature.Models.Request;
using BootCamp.TaskService.Domain.Entity;
using BootCamp.TaskService.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BootCamp.TaskService.Application.TaskCommentFeature.Command;

public class CreateTaskCommentCommand(TaskServiceDbContext context, IOutboxWriter outboxWriter,
     IHttpContextAccessor accessor)
{
    public async Task<BaseResponse> ExecuteAsync
        (CreateTaskCommentRequest request, CancellationToken ct)
    {
        var task = await context.Tasks.Where(x => x.Id == request.TaskId).SingleOrDefaultAsync(ct);

        var taskComment = new TaskComment
        {
            Content = request.Content,
            TaskId = request.TaskId,
            Task = task
        };

        context.TaskComments.Add(taskComment);
        await context.SaveChangesAsync(ct);

        var correlationId = accessor.HttpContext?.Request.Headers["X-Correlation-Id"].FirstOrDefault()
                            ?? Guid.NewGuid().ToString();

        var taskCommentCreated = new TaskCommentCreatedV1(
        TaskId: task.Id,
        Content: taskComment.Content,
        CreateAt: DateTime.UtcNow,
        CorrelationId: correlationId
        );

        await outboxWriter.AddAsync(type: nameof(FirstCommentTaskCreatedV1), payload: taskCommentCreated, correlationId: correlationId, ct);

        return new() { Id = taskComment.Id };
    }
}
