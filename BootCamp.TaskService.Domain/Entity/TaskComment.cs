using System.ComponentModel.DataAnnotations;

namespace BootCamp.TaskService.Domain.Entity;

public class TaskComment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Content { get; set; }
    public Guid TaskId { get; set; }
    public UserTask? Task { get; set; }
    public bool IsDeleted { get; set; }

    [Timestamp]
    public uint RowVersion { get; set; }
}
