using BootCamp.Domain;
using Microsoft.EntityFrameworkCore;

namespace BootCamp.Infrastruture;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
     
    public DbSet<User> Users { get; set; }
    public DbSet<UserTask> Tasks { get; set; }
    public DbSet<TaskComment> TaskComments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserTask>(b=> 
            b.Property(t => t.RowVersion).IsRowVersion().HasColumnName("xmin").IsConcurrencyToken()
        );

        modelBuilder.Entity<UserTask>().HasQueryFilter(x=> !x.IsDeleted);
    }
}
