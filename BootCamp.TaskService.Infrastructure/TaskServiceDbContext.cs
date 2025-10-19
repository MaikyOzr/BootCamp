using BootCamp.TaskService.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace BootCamp.TaskService.Infrastructure;

public sealed class TaskServiceDbContext : DbContext
{
    public TaskServiceDbContext(DbContextOptions<TaskServiceDbContext> options) : base(options) { }

    public DbSet<UserTask> Tasks { get; set; }

    public DbSet<TaskComment> TaskComments { get; set; }

    public DbSet<OutboxEvent> OutboxEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserTask>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<TaskComment>().HasQueryFilter(x => !x.IsDeleted);

        modelBuilder.Entity<OutboxEvent>(b =>
        {
            b.ToTable("OutboxEvents");
            b.HasKey(x => x.Id);
            b.Property(x => x.Type).IsRequired();
            b.Property(x => x.Payload).IsRequired();
            b.Property(x => x.CorrelationId).HasMaxLength(100);
        });
    }
}
