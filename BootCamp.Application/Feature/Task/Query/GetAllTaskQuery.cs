using BootCamp.Application.Feature.Task.Models.Response;
using BootCamp.Infrastruture;
using Microsoft.EntityFrameworkCore;

namespace BootCamp.Application.Feature.Task.Query;

public class GetAllTaskQuery(AppDbContext context)
{
    public async Task<List<GetAllTaskResponse>> ExecuteAsync(Guid userId, CancellationToken ct)
    {
        var task = await context.Tasks.Where(x=> x.UserId == userId)
            .Include(t => t.Comments)
            .Select(t => new GetAllTaskResponse
            {
                Comments = t.Comments,
                Description = t.Description,
                Title = t.Title,
                UserId = t.UserId,
            })
            .AsNoTracking()
            .ToListAsync(ct);

        return task;
    }
}
