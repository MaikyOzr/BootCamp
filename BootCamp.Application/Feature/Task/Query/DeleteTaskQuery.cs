using BootCamp.Infrastruture;
using Microsoft.EntityFrameworkCore;

namespace BootCamp.Application.Feature.Task.Query;

public class DeleteTaskQuery(AppDbContext context)
{
    public async Task<string> ExecuteAsync(Guid id, CancellationToken ct)
    {
        var task = await context.Tasks.Where(x=> x.Id == id).Include(x=> x.Comments)
            .FirstOrDefaultAsync(ct) ?? throw new Exception("Not Found");
        
        task.IsDeleted = true;
        await context.SaveChangesAsync(ct);
        return "Delete success!";
    }
}
