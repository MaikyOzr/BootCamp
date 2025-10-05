using BootCamp.Application.Feature.BaseResponse;
using BootCamp.Application.Feature.Task.Models.Request;
using BootCamp.Domain;
using BootCamp.Infrastruture;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace BootCamp.Application.Feature.Task.Command;

public class CreateTaskWithFirstCommentCommand(AppDbContext context)
{
    public async Task<BaseApiResponse> ExecuteAsync(CreateTaskWithFirstCommentRequest request, CancellationToken ct) 
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
                return new() { Id = task.Id };

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
