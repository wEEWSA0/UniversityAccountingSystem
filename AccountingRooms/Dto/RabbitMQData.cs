using AccountingRooms.Model;

namespace AccountingRooms.Dto;

public class RabbitMQData
{
    public BuildingFromRabbit Building { get; set; }
    public RabbitMQAction Action { get; set; }
}
