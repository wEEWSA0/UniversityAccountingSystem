using RabbitMQ.Client;

using System.Text;
using System.Text.Json;

namespace AccountingBuildings.RabbitMQ;

public class RabbitMQService : IRabbitMQService
{
    private ConnectionFactory _connection;

    private const string Queue = "BuildingsActionsQueue";
    private const string Host = "localhost";

    //public RabbitMQService(string hostName)   todo implemenat with params in AddScoped<>("", "")
    //{
    //    _connection = new ConnectionFactory { HostName = hostName };
    //}

    public RabbitMQService()
    {
        _connection = new ConnectionFactory
        {
            HostName = Host,
            Port = 5672,
            UserName = "guest",
            Password = "guest"
        };
    }

    public void SendMessage(string message)
    {
        using (var connection = _connection.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: Queue,
                           durable: false,
                           exclusive: false,
                           autoDelete: false,
                           arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "",
                           routingKey: Queue,
                           basicProperties: null,
                           body: body);
        }
    }

    public void SendMessage<T>(T message)
    {
        var messageSerialized = JsonSerializer.Serialize(message);
        SendMessage(messageSerialized);
    }
}
