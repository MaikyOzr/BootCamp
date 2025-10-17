using BootCamp.TaskService.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BootCamp.TaskService.Application.TaskCommentFeature.Query
{
    public class DeleteTaskCommentQuery(TaskServiceDbContext context)
    {
        public async Task<bool> ExecuteAsync(Guid id, CancellationToken ct)
        {
            var taskComment = await context.TaskComments.FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new Exception("Not Found");
            taskComment.IsDeleted = true;
            await context.SaveChangesAsync(ct);
            return true;
        }
    }
}
