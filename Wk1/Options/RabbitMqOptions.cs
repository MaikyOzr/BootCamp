namespace Wk1.Options;

public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";
    public required string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public required string UserName { get; set; } = "guest";
    public required string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";
    public string Exchange { get; set; } = "";
}
