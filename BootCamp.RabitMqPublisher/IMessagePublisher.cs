namespace BootCamp.RabitMqPublisher;

public interface IMessagePublisher
{
    Task PublishAsync<T>(string queueName, T message, CancellationToken ct);
}
