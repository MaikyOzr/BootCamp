using BootCamp.TaskService.Application.TaskFeature.Models.Response;
using BootCamp.TaskService.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BootCamp.TaskService.Application.TaskFeature.Query;

public class GetAllTaskQuery(TaskServiceDbContext context)
{
    public async Task<List<GetAllTaskResponse>> ExecuteAsync(Guid userId, CancellationToken ct)
    {
        var task = await context.Tasks.Where(x => x.UserId == userId)
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
