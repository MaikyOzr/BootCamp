using BootCamp.TaskService.Application.TaskFeature.Models.Response;
using BootCamp.TaskService.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BootCamp.TaskService.Application.TaskFeature.Query;

public class GetByIdTaskQuery(TaskServiceDbContext context)
{
    public async Task<GetByIdTaskResponse> ExecuteAsync(Guid id, CancellationToken ct)
    {
        var task = await context.Tasks.Where(x=> x.Id == id)
            .Include(x => x.Comments)
            .Select(x => new GetByIdTaskResponse
            {
                Comments = x.Comments,
                Description = x.Description,
                Title = x.Title,
                UserId = x.UserId,
                RowVersion = x.RowVersion
            })
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

        return task;
    }
}
