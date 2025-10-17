using BootCamp.TaskService.Application.TaskCommentFeature.Models.Response;
using BootCamp.TaskService.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BootCamp.TaskService.Application.TaskCommentFeature.Query
{
    public class GetByIdTaskCommentQuery(TaskServiceDbContext context)
    {
        public async Task<GetByIdTaskCommentResponse> ExecuteAsync(Guid id, CancellationToken ct)
        {
            var taskComment = await context.TaskComments
                .Where(x => x.Id == id)
                .AsNoTracking().FirstOrDefaultAsync(ct);

            return new()
            {
                Content = taskComment.Content,
                TaskId = taskComment.TaskId,
                RowVersion = taskComment.RowVersion,
            };
        }
    }
}
