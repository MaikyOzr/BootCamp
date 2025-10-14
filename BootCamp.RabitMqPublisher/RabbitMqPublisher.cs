using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace BootCamp.RabitMqPublisher;

public class RabbitMqPublisher(IConnection connection) : IMessagePublisher
{
    private const string MainExchange = "app.exchange";
    private const string RetryExchange = "app.retry";
    private const string DlxExchange = "app.dlx";

    private const string DlqQueue = "task.queue.dlq";
    private const string DlqRoutingKey = "task.dead";

    private const string RetryKey1 = "task.retry.1";
    private const string RetryKey3 = "task.retry.3";
    private const string RetryKey5 = "task.retry.5";

    private const string RetryQueue1 = "task.retry.1s";
    private const string RetryQueue3 = "task.retry.3s";
    private const string RetryQueue5 = "task.retry.5s";

    public async Task PublishAsync<T>(string mainQueueName, T message, CancellationToken ct)
    {
        string MainRoutingKey = mainQueueName + ".created";

        Dictionary<string, object> args = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", DlxExchange },
            { "x-dead-letter-routing-key", DlqRoutingKey }
        };

        await using var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(MainExchange, "topic", durable: true);
        await channel.ExchangeDeclareAsync(RetryExchange, "direct", durable: true);
        await channel.ExchangeDeclareAsync(DlxExchange, "direct", durable: true);

        //Main Queue
        await channel.QueueDeclareAsync(
            queue: mainQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: args,
            cancellationToken: ct);

        //Main Binding
        await channel.QueueBindAsync(
            queue: mainQueueName,
            exchange: MainExchange,
            routingKey: MainRoutingKey,
            cancellationToken: ct); 
        
        // Start Try DLX and DLQ
        await channel.QueueDeclareAsync(
            queue: DlqQueue, 
            durable: true,
            exclusive: false, 
            autoDelete: false, arguments: null);

        await channel.QueueBindAsync(
            queue: DlqQueue, exchange: DlxExchange, routingKey: DlqRoutingKey);

        await channel.QueueDeclareAsync(
            queue: RetryQueue1,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", MainExchange },
                { "x-dead-letter-routing-key", MainRoutingKey },
                { "x-message-ttl", 1000 }
            });

        await channel.QueueDeclareAsync(
            queue: RetryQueue3,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", MainExchange },
                { "x-dead-letter-routing-key", MainRoutingKey },
                { "x-message-ttl", 3000 }
            });

        await channel.QueueDeclareAsync(
            queue: RetryQueue5,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", MainExchange },
                { "x-dead-letter-routing-key", MainRoutingKey },
                { "x-message-ttl", 5000 }
            });

        await channel.QueueBindAsync(
            queue: RetryQueue1,
            exchange: RetryExchange,
            routingKey: RetryKey1);

        await channel.QueueBindAsync(
            queue: RetryQueue3,
            exchange: RetryExchange,
            routingKey: RetryKey3);

        await channel.QueueBindAsync(
            queue: RetryQueue5,
            exchange: RetryExchange,
            routingKey: RetryKey5);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        var props = new BasicProperties { Persistent = true };
        props.ContentType = "application/json";
        props.DeliveryMode = DeliveryModes.Persistent;

        await channel.BasicPublishAsync(
            exchange: MainExchange,
            routingKey: MainRoutingKey,
            mandatory: true,
            props,
            body,
            ct);

    }
}
