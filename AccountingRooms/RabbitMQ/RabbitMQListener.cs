using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System.Text;

namespace AccountingRooms.RabbitMQ;

public class RabbitMqListener : BackgroundService
{
    private IConnection _connection;
    private IModel _channel;

    private RabbitMQHandler _handler;

    // todo move to config
    private const string Queue = "BuildingsActionsQueue";
    private const string Host = "localhost";

    public RabbitMqListener(IServiceProvider serviceProvider)
    {
        var factory = new ConnectionFactory {
            HostName = Host,
            Port = 5672,
            UserName = "guest",
            Password = "guest"
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: Queue, durable: false, exclusive: false, autoDelete: false, arguments: null);

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            RabbitMQHandler scopedProcessingService =
                scope.ServiceProvider.GetRequiredService<RabbitMQHandler>();

            _handler = scopedProcessingService;
        }
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (ch, ea) =>
        {
            var content = Encoding.UTF8.GetString(ea.Body.ToArray());

            Console.WriteLine($"Получено сообщение: {content}");

            _handler.Process(content);

            _channel.BasicAck(ea.DeliveryTag, false);
        };

        _channel.BasicConsume(Queue, false, consumer);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }
}
