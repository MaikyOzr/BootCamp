using BootCamp.Application.Feature.TaskCommentFeature.Models.Response;
using BootCamp.Infrastruture;
using Microsoft.EntityFrameworkCore;

namespace BootCamp.Application.Feature.TaskCommentFeature.Query;

public class GetByIdTaskCommentQuery(AppDbContext context)
{
    public async Task<GetByIdTaskCommentResponse> ExecuteAsync(Guid id, CancellationToken ct)
    {
        var taskComment = await context.TaskComments
            .Where(x=> x.Id == id)
            .AsNoTracking().FirstOrDefaultAsync(ct);

        return new() 
        {
            Content = taskComment.Content,
            TaskId = taskComment.TaskId
        };
    }
}
