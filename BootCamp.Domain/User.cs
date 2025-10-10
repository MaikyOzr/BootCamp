using Microsoft.AspNetCore.Identity;

namespace BootCamp.Domain;


public class User : IdentityUser<Guid>
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public List<UserTask> Tasks { get; set; } = new();
}
