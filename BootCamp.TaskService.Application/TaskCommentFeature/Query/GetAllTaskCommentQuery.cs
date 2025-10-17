using BootCamp.TaskService.Application.TaskCommentFeature.Models.Response;
using BootCamp.TaskService.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BootCamp.TaskService.Application.TaskCommentFeature.Query
{
    public class GetAllTaskCommentQuery(TaskServiceDbContext context)
    {
        public async Task<List<GetAllTaskCommentResponse>> ExecuteAsync(Guid id, CancellationToken ct)
        {
            var taskComments = await context.TaskComments
                .Where(x => x.TaskId == id)
                .Select(x => new GetAllTaskCommentResponse
                {
                    Content = x.Content,
                    TaskId = x.TaskId,
                    RowVersion = x.RowVersion,
                })
                .AsNoTracking()
                .ToListAsync(ct);

            return taskComments;
        }
    }
}
