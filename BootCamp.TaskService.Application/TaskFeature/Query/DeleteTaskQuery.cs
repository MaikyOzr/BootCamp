using BootCamp.Contract.Events;
using BootCamp.RabitMqPublisher;
using BootCamp.TaskService.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BootCamp.TaskService.Application.TaskFeature.Query;

public class DeleteTaskQuery(TaskServiceDbContext context, IMessagePublisher publisher,
     IHttpContextAccessor accessor)
{
    public async Task<string> ExecuteAsync(Guid id, CancellationToken ct)
    {
        var task = await context.Tasks.Where(x=> x.Id == id).Include(x=> x.Comments)
            .FirstOrDefaultAsync(ct) ?? throw new Exception("Not Found");
        
        task.IsDeleted = true;
        await context.SaveChangesAsync(ct);

        var correlationId = accessor.HttpContext?.Request.Headers["X-Correlation-Id"].FirstOrDefault()
                         ?? Guid.NewGuid().ToString();

        var @event = new TaskDeletedV1(
                TaskId: task.Id,
                DeletedAt: DateTime.UtcNow,
                CorrelationId: correlationId
            );

        await publisher.PublishAsync("task.queue", @event, ct);

        return "Delete success!";
    }
}
