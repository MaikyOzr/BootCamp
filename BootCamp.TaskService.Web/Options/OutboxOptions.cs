namespace BootCamp.TaskService.Web.Background;

public sealed class OutboxOptions
{
    public const string SectionName = "Outbox";

    public int BatchSize { get; set; } = 50;
    public int PollIntervalMs { get; set; } = 1000;
    public int MaxAttempts { get; set; } = 5;
    public int MaxBackoffMs { get; set; } = 30_000;
}
