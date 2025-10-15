using BootCamp.TaskService.Application.TaskFeature.Models.Request;
using BootCamp.TaskService.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BootCamp.TaskService.Application.TaskFeature.Command;

public class UpdateTaskCommand(TaskServiceDbContext context)
{
    public async Task<BaseResponse> ExecuteAsync(Guid id, UpdateTaskRequest request, CancellationToken ct)
    {
        var task = await context.Tasks.SingleOrDefaultAsync(x => x.Id == id);

        task.Title = request.Title;
        task.Description = request.Description;

        try
        {
            await context.SaveChangesAsync(ct);
            return new() { Id = task.Id };
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new Exception("A concurrency error occurred while updating the task.", ex);
        }
    }
}
