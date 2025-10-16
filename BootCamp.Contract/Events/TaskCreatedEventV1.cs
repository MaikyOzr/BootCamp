namespace BootCamp.Contract.Events;

public record TaskCreatedEventV1(
    Guid TaskId,
    Guid UserId,
    string Title,
    DateTime CreatedAt,
    string CorrelationId
);
