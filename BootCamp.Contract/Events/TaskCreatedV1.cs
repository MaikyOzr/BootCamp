namespace BootCamp.Contract.Events;

public record TaskCreatedV1(
    Guid TaskId,
    Guid UserId,
    string Title,
    string? Description,
    DateTime CreatedAt,
    string CorrelationId
);
