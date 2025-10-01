using BootCamp.Domain;
using Microsoft.EntityFrameworkCore;

namespace BootCamp.Infrastruture;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
     
    public DbSet<User> Users { get; set; }
    public DbSet<Domain.Task> Tasks { get; set; }
    public DbSet<TaskComment> TaskComments { get; set; }
}
