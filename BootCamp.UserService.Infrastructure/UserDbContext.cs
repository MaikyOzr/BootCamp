using BootCamp.UserService.Domain;
using Microsoft.EntityFrameworkCore;

namespace BootCamp.UserService.Database;

public sealed class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
}
