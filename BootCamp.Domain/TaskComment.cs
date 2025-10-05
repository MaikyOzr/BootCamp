namespace BootCamp.Domain;

public class TaskComment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Content { get; set; }
    public Guid TaskId { get; set; }
    public UserTask? Task { get; set; }
}
