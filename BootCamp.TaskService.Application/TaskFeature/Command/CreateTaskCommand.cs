using BootCamp.Contract.Events;
using BootCamp.RabitMqPublisher;
using BootCamp.TaskService.Application.Common;
using BootCamp.TaskService.Application.TaskFeature.Models.Request;
using BootCamp.TaskService.Domain.Entity;
using BootCamp.TaskService.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BootCamp.TaskService.Application.TaskFeature.Command;

public class CreateTaskCommand(TaskServiceDbContext context, IMessagePublisher publisher, 
     IHttpContextAccessor accessor, IOutboxWriter outboxWriter)
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

        try
        {
            context.Tasks.Add(task);
            await context.SaveChangesAsync(ct);

            var correlationId = accessor.HttpContext?.Request.Headers["X-Correlation-Id"].FirstOrDefault()
                            ?? Guid.NewGuid().ToString();

            var createdTask = new TaskCreatedV1(
                TaskId: task.Id,
                UserId: task.UserId,
                Title: task.Title,
                Description: task.Description,
                CreatedAt: DateTime.UtcNow,
                CorrelationId: correlationId
            );

            await outboxWriter.AddAsync(type: nameof(TaskCreatedV1), payload: createdTask, correlationId: correlationId, ct);

            return new() { Id = task.Id };
        }
        catch (DbUpdateConcurrencyException ex) 
        {
            throw new Exception("A concurrency error occurred while creating the task.", ex);
        }
    }
}
