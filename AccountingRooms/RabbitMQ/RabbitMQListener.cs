using Microsoft.Extensions.Options;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System.Text;
using System.Text.Json;

namespace AccountingRooms.RabbitMQ;

public class RabbitMqListener : BackgroundService, IRabbitMQListener
{
    private IConnection _connection;
    private IModel _channel;
    private IServiceProvider _serviceProvider;

    private string _queue { get; init; }

    public RabbitMqListener(IServiceProvider serviceProvider, IOptions<RabbitMQOptions> rabbitMQListenerOptions)
    {
        var options = rabbitMQListenerOptions.Value!;

        _queue = options.Queue;
        Console.WriteLine("RabbitMQOptions: " + JsonSerializer.Serialize(options));

        try
        {
            ConnectionFactory factory = new()
            {
                HostName = options.HostName,
                Port = options.Port,
                UserName = options.UserName,
                Password = options.Password
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _queue, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (ch, ea) =>
        {
            var content = Encoding.UTF8.GetString(ea.Body.ToArray());

            Console.WriteLine($"Получено сообщение: {content}");

            try
            {
                using (IServiceScope scope = _serviceProvider.CreateScope())
                {
                    RabbitMQHandler scopedProcessingService =
                        scope.ServiceProvider.GetRequiredService<RabbitMQHandler>();

                    var handler = scopedProcessingService;

                    handler.Process(content);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                throw;
            }

            _channel.BasicAck(ea.DeliveryTag, false);
        };

        _channel.BasicConsume(_queue, false, consumer);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }
}
