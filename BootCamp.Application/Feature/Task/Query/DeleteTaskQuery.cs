using BootCamp.Infrastruture;
using Microsoft.EntityFrameworkCore;

namespace BootCamp.Application.Feature.Task.Query;

public class DeleteTaskQuery(AppDbContext context)
{
    public async ValueTask ExecuteAsync(Guid id, CancellationToken ct)
    {
        var task = context.Tasks.Where(x=> x.Id == id).Include(x=> x.Comments);
        if (task != null)
        {
            context.Tasks.Remove((Domain.Task)task);
            await context.SaveChangesAsync(ct);
        }
    }
}
