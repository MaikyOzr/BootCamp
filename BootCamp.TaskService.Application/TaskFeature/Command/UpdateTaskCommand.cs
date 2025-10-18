using BootCamp.Contract.Events;
using BootCamp.RabitMqPublisher;
using BootCamp.TaskService.Application.Common;
using BootCamp.TaskService.Application.TaskFeature.Models.Request;
using BootCamp.TaskService.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BootCamp.TaskService.Application.TaskFeature.Command;

public class UpdateTaskCommand(TaskServiceDbContext context, IMessagePublisher publisher,
     IHttpContextAccessor accessor, IOutboxWriter outboxWriter)
{
    public async Task<BaseResponse> ExecuteAsync(Guid id, UpdateTaskRequest request, CancellationToken ct)
    {
        var task = await context.Tasks.FirstOrDefaultAsync(x => x.Id == id, ct);

        task.Title = request.Title;
        task.Description = request.Description;

        try
        {
            await context.SaveChangesAsync(ct);
            var correlationId = accessor.HttpContext?.Request.Headers["X-Correlation-Id"].FirstOrDefault()
                        ?? Guid.NewGuid().ToString();

            var @event = new TaskUpdatedV1(
                TaskId: task.Id,
                UserId: task.UserId,
                Title: task.Title,
                Description: task.Description,
                UpdatedAt: DateTime.UtcNow,
                CorrelationId: correlationId
            );

            

            return new() { Id = task.Id };
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new Exception("A concurrency error occurred while updating the task.", ex);
        }
    }
}
