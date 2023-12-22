namespace AccountingBuildings.RabbitMQ;

public interface IRabbitMQService
{
    void SendMessage(string message);
    void SendMessage<T>(T message);
}
