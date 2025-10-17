using BootCamp.Contract.Events;
using BootCamp.RabitMqPublisher;
using BootCamp.TaskService.Application.TaskFeature.Models.Request;
using BootCamp.TaskService.Domain.Entity;
using BootCamp.TaskService.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace BootCamp.TaskService.Application.TaskFeature.Command;

public class CreateTaskWithFirstCommentCommand(TaskServiceDbContext context, IMessagePublisher publisher,
     IHttpContextAccessor accessor)
{
    public async Task<BaseResponse> ExecuteAsync(CreateTaskWithFirstCommentRequest request, CancellationToken ct) 
    {
        var taskComment = new TaskComment() { Content = request.TaskComment };

        var task = new UserTask()
        {
            Title = request.TaskTitle,
            Description = request.TaskDescription,
            UserId = request.UserId,
            Comments = new List<TaskComment> { taskComment }
        };

        var existTask = await context.Tasks.Where(x=> x.UserId == task.UserId).SingleOrDefaultAsync(ct);
        if (existTask.Title == task.Title)
        {
            throw new Exception("Same task is exist");
        }

        using (var scope = new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
        {
            try
            {
                context.Tasks.Add(task);
                await context.SaveChangesAsync(ct);
                scope.Complete();
                var correlationId = accessor.HttpContext?.Request.Headers["X-Correlation-Id"].FirstOrDefault()
                            ?? Guid.NewGuid().ToString();
                
                var @event = new FirstCommentTaskCreatedV1(
                TaskId: task.Id,
                UserId: task.UserId,
                TaskTitle: task.Title,
                TaskDescription: task.Description,
                TaskComment: taskComment.Content,
                CreatedAt: DateTime.UtcNow,
                CorrelationId: correlationId
                );

                await publisher.PublishAsync("task.queue", @event, ct);

                return new() { Id = task.Id };

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
