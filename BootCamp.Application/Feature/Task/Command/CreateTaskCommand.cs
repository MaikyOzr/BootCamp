using BootCamp.Application.Feature.BaseResponse;
using BootCamp.Application.Feature.Task.Models.Request;
using BootCamp.Domain;
using BootCamp.Infrastruture;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BootCamp.Application.Feature.Task.Command;

public class CreateTaskCommand(AppDbContext context, UserManager<User> userManager)
{
    public async Task<BaseApiResponse> ExecuteAsync(CreateTaskRequest request, CancellationToken ct)
    {
        var existUser = await userManager.FindByIdAsync(request.UserId.ToString());

        var task = new UserTask
        {
            Title = request.Title,
            Description = request.Description,
            UserId = existUser.Id,
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
