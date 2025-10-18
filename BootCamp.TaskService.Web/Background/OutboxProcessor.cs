using BootCamp.Contract.Events;
using BootCamp.RabitMqPublisher;
using BootCamp.TaskService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace BootCamp.TaskService.Web.Background;

public class OutboxProcessor(IServiceProvider services, 
    ILogger<OutboxProcessor> logger, IOptions<OutboxOptions> options) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("OutboxProcessor started. BatchSize={BatchSize}, PollIntervalMs={PollMs}, MaxAttempts={MaxAttempts}",
            options.Value.BatchSize, options.Value.PollIntervalMs, options.Value.MaxAttempts);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<TaskServiceDbContext>();
                var publisher = scope.ServiceProvider.GetRequiredService<IMessagePublisher>();

                var events = await db.OutboxEvents
                    .Where(e => e.ProcessedAtUtc == null && e.Attempts < options.Value.MaxAttempts)
                    .OrderBy(e => e.OccurredAtUtc)
                    .Take(options.Value.BatchSize)
                    .ToListAsync(stoppingToken);

                if (events.Count == 0)
                {
                    await Task.Delay(options.Value.PollIntervalMs, stoppingToken);
                    continue;
                }

                foreach (var e in events)
                {
                    try
                    {
                        switch (e.Type)
                        {
                            case nameof(TaskCreatedV1):
                                {
                                    var msg = JsonSerializer.Deserialize<TaskCreatedV1>(e.Payload);
                                    if (msg is null)
                                    {
                                        logger.LogWarning("Outbox event {Id} payload deserialized to null for type {Type}", e.Id, e.Type);
                                        e.Attempts++;
                                        break;
                                    }
                                    await publisher.PublishAsync("task", msg, stoppingToken);

                                    e.ProcessedAtUtc = DateTime.UtcNow;
                                    break;
                                }

                            default:
                                logger.LogWarning("Unknown outbox event type {Type}, id {Id}", e.Type, e.Id);
                                e.Attempts++;
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        e.Attempts++;
                        var backoffMs = ComputeBackoffMs(e.Attempts, options);
                        logger.LogError(ex, "Failed to publish OutboxEvent {Id}. Attempts={Attempts}. Backing off {BackoffMs}ms",
                            e.Id, e.Attempts, backoffMs);
                    }
                }

                await db.SaveChangesAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // graceful shutdown
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled error in OutboxProcessor loop");
            }

            await Task.Delay(options.Value.PollIntervalMs, stoppingToken);
        }

        logger.LogInformation("OutboxProcessor stopped.");
    }

    private static int ComputeBackoffMs(int attempts, IOptions<OutboxOptions> opts)
    {
        var ms = (int)Math.Min(opts.Value.PollIntervalMs * Math.Pow(2, attempts - 1), opts.Value.MaxBackoffMs);
        return Math.Max(ms, opts.Value.PollIntervalMs);
    }
}
