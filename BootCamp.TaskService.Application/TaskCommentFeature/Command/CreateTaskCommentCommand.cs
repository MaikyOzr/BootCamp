using BootCamp.TaskService.Application.TaskCommentFeature.Models.Request;
using BootCamp.TaskService.Domain.Entity;
using BootCamp.TaskService.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BootCamp.TaskService.Application.TaskCommentFeature.Command;

public class CreateTaskCommentCommand(TaskServiceDbContext context)
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

        return new() { Id = taskComment.Id };
    }
}
