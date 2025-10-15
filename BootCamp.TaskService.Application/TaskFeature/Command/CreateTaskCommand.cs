using BootCamp.TaskService.Application.TaskFeature.Models.Request;
using BootCamp.TaskService.Domain.Entity;
using BootCamp.TaskService.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BootCamp.TaskService.Application.TaskFeature.Command;

public class CreateTaskCommand(TaskServiceDbContext context)
{
    public async Task<BaseResponse> ExecuteAsync(CreateTaskRequest request, CancellationToken ct)
    {
        var task = new UserTask
        {
            Title = request.Title,
            Description = request.Description,
            UserId = request.UserId,
        };

        var existTask = await context.Tasks.Where(x => x.UserId == task.UserId).FirstOrDefaultAsync(ct);
        if (existTask != null && existTask.Title == task.Title)
        {
            throw new Exception("Same task is exist");
        }

        context.Tasks.Add(task);

        try
        {
            await context.SaveChangesAsync(ct);

            return new() { Id = task.Id };
        }
        catch (DbUpdateConcurrencyException ex) 
        {
            throw new Exception("A concurrency error occurred while creating the task.", ex);
        }
    }
}
