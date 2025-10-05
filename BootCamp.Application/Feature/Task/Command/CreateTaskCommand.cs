using BootCamp.Application.Feature.BaseResponse;
using BootCamp.Application.Feature.Task.Models.Request;
using BootCamp.Infrastruture;
using Microsoft.EntityFrameworkCore;

namespace BootCamp.Application.Feature.Task.Command;

public class CreateTaskCommand(AppDbContext context)
{
    public async Task<BaseApiResponse> ExecuteAsync(CreateTaskRequest request, CancellationToken ct)
    {
        var task = new Domain.UserTask
        {
            Title = request.Title,
            Description = request.Description,
            UserId = request.UserId
        };

        var existTask = await context.Tasks.Where(x => x.UserId == task.UserId).SingleOrDefaultAsync(ct);
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
