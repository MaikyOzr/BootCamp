namespace BootCamp.Contract.Events;

public record TaskCommentUpdatedV1(
    Guid TaskId,
    string Content,
    DateTime CreateAt,
    string CorrelationId
    );
