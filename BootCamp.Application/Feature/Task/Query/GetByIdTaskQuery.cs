using BootCamp.Application.Feature.Task.Models.Response;
using BootCamp.Infrastruture;
using Microsoft.EntityFrameworkCore;

namespace BootCamp.Application.Feature.Task.Query;

public class GetByIdTaskQuery(AppDbContext context)
{
    public async Task<GetByIdTaskResponse> ExecuteAsync(Guid id, CancellationToken ct)
    {
        var task = await context.Tasks.Where(x=> x.Id == id).Include(x => x.Comments)
            .Select(t => new GetByIdTaskResponse
            {
                Comments = t.Comments,
                Description = t.Description,
                Title = t.Title,
                UserId = t.UserId,
            })
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

        return task;
    }
}
