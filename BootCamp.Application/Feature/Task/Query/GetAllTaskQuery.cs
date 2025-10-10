using BootCamp.Application.Feature.Task.Models.Response;
using BootCamp.Domain;
using BootCamp.Infrastruture;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BootCamp.Application.Feature.Task.Query;

public class GetAllTaskQuery(AppDbContext context)
{
    public async Task<List<GetAllTaskResponse>> ExecuteAsync(Guid userId, CancellationToken ct)
    {
        var task = await context.Tasks.Where(x=> x.UserId == userId)
            .Include(x => x.Comments)
            .Select(x => new GetAllTaskResponse
            {
                Comments = x.Comments,
                Description = x.Description,
                Title = x.Title,
                UserId = x.UserId,
                RowVersion = x.RowVersion,
            })
            .AsNoTracking()
            .ToListAsync(ct);

        return task;
    }
}
