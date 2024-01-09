namespace AccountingRooms.RabbitMQ;

public class RabbitMQOptions
{
    public string HostName { get; set; } = null!;
    public int Port { get; set; }
    public string Queue { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
}
