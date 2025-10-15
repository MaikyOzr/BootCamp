using System.ComponentModel.DataAnnotations;

namespace BootCamp.TaskService.Domain.Entity;

public class UserTask
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Title { get; set; }
    public string? Description { get; set; }
    public Guid UserId { get; set; }
    public List<TaskComment> Comments { get; set; } = [];

    [Timestamp]
    public uint RowVersion { get; set; }

    public bool IsDeleted { get; set; }

}
