namespace BootCamp.Contract.Events;

public record TaskCommentCreatedV1(
    Guid TaskId,
    string Content,
    DateTime CreateAt,
    string CorrelationId
    );
