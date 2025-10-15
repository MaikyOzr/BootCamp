namespace BootCamp.Contract.Events;

public sealed record TaskCreatedEventV1(
    Guid TaskId,
    Guid UserId,
    string Title,
    DateTime CreatedAtUtc,
    string CorrelationId
);
