namespace BootCamp.Contract.Events;

public record TaskUpdatedV1(string? Title, 
    string? Description, Guid TaskId, 
    string CorrelationId, Guid UserId, DateTime UpdatedAt);
