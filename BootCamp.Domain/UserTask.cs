using System.ComponentModel.DataAnnotations;

namespace BootCamp.Domain;

public class UserTask
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Title { get; set; }
    public string? Description { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public List<TaskComment> Comments { get; set; } = [];

    [Timestamp]
    public uint RowVersion { get; set; }

}
