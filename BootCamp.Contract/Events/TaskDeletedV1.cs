namespace BootCamp.Contract.Events;

public record TaskDeletedV1(
    Guid TaskId,
    DateTime DeletedAt,
    string CorrelationId
);

