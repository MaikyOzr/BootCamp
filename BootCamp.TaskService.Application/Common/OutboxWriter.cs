
using BootCamp.TaskService.Domain.Entity;
using BootCamp.TaskService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BootCamp.TaskService.Application.Common;

public class OutboxWriter(TaskServiceDbContext context) : IOutboxWriter
{
    public async Task AddAsync(string type, object payload, string correlationId, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(payload);

        var outboxEvent = new OutboxEvent
        {
            Id = Guid.NewGuid(),
            Type = type,
            Payload = json,
            CorrelationId = correlationId,
            OccurredAt = DateTime.UtcNow
        };

        context.OutboxEvents.Add(outboxEvent);
        await context.SaveChangesAsync(ct);

    }
}
