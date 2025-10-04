using BootCamp.Application.Feature.TaskCommentFeature.Models.Response;
using BootCamp.Infrastruture;
using Microsoft.EntityFrameworkCore;

namespace BootCamp.Application.Feature.TaskCommentFeature.Query;

public class GetAllTaskCommentQuery(AppDbContext context)
{
    public async Task<List<GetAllTaskCommentResponse>> ExecuteAsync(Guid id, CancellationToken ct)
    {
        var taskComments = await context.TaskComments
            .Where(x => x.TaskId == id)
            .Select(tc => new GetAllTaskCommentResponse
            {
                Content = tc.Content,
                TaskId = tc.TaskId
            })
            .AsNoTracking()
            .ToListAsync(ct);

        return taskComments;
    }
}
