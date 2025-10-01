using BootCamp.Application.Feature.BaseResponse;
using BootCamp.Application.Feature.Task.Models.Request;
using BootCamp.Infrastruture;

namespace BootCamp.Application.Feature.Task.Command;

public class CreateTaskCommand(AppDbContext context)
{
    public async Task<BaseApiResponse> ExecuteAsync(CreateTaskRequest request, CancellationToken ct)
    {
        var task = new Domain.Task
        {
            Title = request.Title,
            Description = request.Description,
            UserId = request.UserId
        };

        context.Tasks.Add(task);
        await context.SaveChangesAsync(ct);

        return new() { Id = task.Id };
    }
}
