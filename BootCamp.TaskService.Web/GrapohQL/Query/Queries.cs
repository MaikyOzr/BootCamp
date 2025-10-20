using BootCamp.TaskService.Domain.Entity;
using BootCamp.TaskService.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BootCamp.TaskService.Web.GrapohQL.Query;

public sealed class Queries
{
    [GraphQLName("tasks")]
    public IQueryable<UserTask> GetTasks([Service] TaskServiceDbContext context)
        => context.Tasks.AsNoTracking();

    [GraphQLName("task")]
    public async Task<UserTask?> GetTaskByIdAsync(
        [ID] Guid id,
        [Service] TaskServiceDbContext context,
        CancellationToken ct)
        => await context.Tasks.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, ct);
}
