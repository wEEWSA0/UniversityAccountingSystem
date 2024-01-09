using Microsoft.Extensions.Options;

using RabbitMQ.Client;

using System.Text;
using System.Text.Json;

namespace AccountingBuildings.RabbitMQ;

public class RabbitMQService : IRabbitMQService
{
    private ConnectionFactory _connection;

    private string _queue { get; init; }

    public RabbitMQService(IOptions<RabbitMQOptions> rabbitMQListenerOptions)
    {
        var options = rabbitMQListenerOptions.Value!;

        _queue = options.Queue;

        Console.WriteLine(JsonSerializer.Serialize(options));

        _connection = new()
        {
            HostName = options.HostName,
            Port = options.Port,
            UserName = options.UserName,
            Password = options.Password
        };
    }

    public void SendMessage(string message)
    {
        using (var connection = _connection.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: _queue,
                           durable: false,
                           exclusive: false,
                           autoDelete: false,
                           arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "",
                           routingKey: _queue,
                           basicProperties: null,
                           body: body);
        }
    }

    public void SendMessage<T>(T message)
    {
        var messageSerialized = JsonSerializer.Serialize(message);

        Console.WriteLine("Message: " + messageSerialized);

        SendMessage(messageSerialized);
    }
}
