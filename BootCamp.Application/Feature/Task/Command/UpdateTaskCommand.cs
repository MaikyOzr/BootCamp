using BootCamp.Application.Feature.BaseResponse;
using BootCamp.Application.Feature.Task.Models.Request;
using BootCamp.Infrastruture;
using Microsoft.EntityFrameworkCore;

namespace BootCamp.Application.Feature.Task.Command;

public class UpdateTaskCommand(AppDbContext context)
{
    public async Task<BaseApiResponse> ExecuteAsync(Guid id, UpdateTaskRequest request, CancellationToken ct)
    {
        var task = await context.Tasks.SingleOrDefaultAsync(x => x.Id == id);

        task.Title = request.Title;
        task.Description = request.Description;

        await context.SaveChangesAsync(ct);
        return new() { Id = task.Id};
    }
}
