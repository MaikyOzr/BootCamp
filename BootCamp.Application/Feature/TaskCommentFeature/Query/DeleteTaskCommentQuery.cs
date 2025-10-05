using BootCamp.Infrastruture;
using Microsoft.EntityFrameworkCore;

namespace BootCamp.Application.Feature.TaskCommentFeature.Query;

public class DeleteTaskCommentQuery(AppDbContext context)
{
    public async Task<bool> ExecuteAsync(Guid id, CancellationToken ct)
    {
        var taskComment = await context.TaskComments.FirstOrDefaultAsync(x => x.Id == id);
        context.TaskComments.Remove(taskComment);
        await context.SaveChangesAsync(ct);
        return true;
    }
}
