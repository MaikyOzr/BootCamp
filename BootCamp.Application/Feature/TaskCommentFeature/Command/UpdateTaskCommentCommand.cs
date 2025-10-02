using BootCamp.Application.Feature.BaseResponse;
using BootCamp.Application.Feature.TaskCommentFeature.Models.Request;
using BootCamp.Infrastruture;
using Microsoft.EntityFrameworkCore;

namespace BootCamp.Application.Feature.TaskCommentFeature.Command
{
    public class UpdateTaskCommentCommand(AppDbContext context)
    {
        public async Task<BaseApiResponse> ExecuteAsync(Guid id, UpdateTaskCommentRequest request, CancellationToken ct)
        {
            var taskComment = await context.TaskComments.FirstOrDefaultAsync(x => x.Id == id, ct);
            
            taskComment.Content = request.Content;

            await context.SaveChangesAsync(ct);
            return new() { Id = taskComment.Id };
        }
    }
}
