namespace BootCamp.Contract.Events;

public sealed record FirstCommentTaskCreatedV1(
    Guid TaskId, string? TaskTitle, 
    string? TaskDescription, string? TaskComment, 
    Guid UserId, string CorrelationId, DateTime CreatedAt);
