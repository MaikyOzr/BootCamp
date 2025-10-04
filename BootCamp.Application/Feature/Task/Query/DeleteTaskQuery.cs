using BootCamp.Infrastruture;
using Microsoft.EntityFrameworkCore;

namespace BootCamp.Application.Feature.Task.Query;

public class DeleteTaskQuery(AppDbContext context)
{
    public async Task<bool> ExecuteAsync(Guid id, CancellationToken ct)
    {
        var task = await context.Tasks.Where(x=> x.Id == id).Include(x=> x.Comments)
            .SingleOrDefaultAsync(ct);
        context.Tasks.Remove(task);
        await context.SaveChangesAsync(ct);
        return true;
    }
}
