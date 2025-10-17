using BootCamp.TaskService.Application.TaskCommentFeature.Models.Request;
using BootCamp.TaskService.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BootCamp.TaskService.Application.TaskCommentFeature.Command;

public class UpdateTaskCommentCommand(TaskServiceDbContext context)
{
    public async Task<BaseResponse> ExecuteAsync(Guid id, UpdateTaskCommentRequest request, CancellationToken ct)
    {
        var taskComment = await context.TaskComments.Where(x => x.Id == id)
             .SingleOrDefaultAsync(ct);

        taskComment.Content = request.Content;

        await context.SaveChangesAsync(ct);
        return new() { Id = taskComment.Id };
    }
}
