using BootCamp.Infrastruture;

namespace BootCamp.Application.Feature.TaskCommentFeature.Query;

public class DeleteTaskCommentQuery(AppDbContext context)
{
    public async ValueTask ExecuteAsync(Guid id, CancellationToken ct)
    {
        var taskComment = context.TaskComments.FirstOrDefault(x => x.Id == id);
        if (taskComment != null)
        {
            context.TaskComments.Remove(taskComment);
            await context.SaveChangesAsync(ct);
        }
    }
}
