using BootCamp.TaskService.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace BootCamp.TaskService.Infrastructure;

public sealed class TaskServiceDbContext : DbContext
{
    public TaskServiceDbContext(DbContextOptions<TaskServiceDbContext> options) : base(options) { }

    public DbSet<UserTask> Tasks { get; set; }

    public DbSet<TaskComment> TaskComments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserTask>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<TaskComment>().HasQueryFilter(x => !x.IsDeleted);
    }
}
