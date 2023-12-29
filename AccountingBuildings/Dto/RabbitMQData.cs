using AccountingBuildings.Model;

namespace AccountingBuildings.Dto;

public class RabbitMQData
{
    public Building Building { get; set; }
    public RabbitMQAction Action { get; set; }
}
