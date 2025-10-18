namespace BootCamp.TaskService.Application.Common;

public interface IOutboxWriter
{
    Task AddAsync(string type, object payload, string correlationId, CancellationToken ct);
}
