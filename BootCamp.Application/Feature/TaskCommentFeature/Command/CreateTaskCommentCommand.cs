using BootCamp.Application.Feature.BaseResponse;
using BootCamp.Application.Feature.TaskCommentFeature.Models.Request;
using BootCamp.Domain;
using BootCamp.Infrastruture;
using Microsoft.EntityFrameworkCore;

namespace BootCamp.Application.Feature.TaskCommentFeature.Command;

public class CreateTaskCommentCommand(AppDbContext context)
{
    public async Task<BaseApiResponse> ExecuteAsync
        (CreateTaskCommentRequest request, CancellationToken ct)
    {
        var task = await context.Tasks.Where(x=> x.Id == request.TaskId).FirstOrDefaultAsync(ct);

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
