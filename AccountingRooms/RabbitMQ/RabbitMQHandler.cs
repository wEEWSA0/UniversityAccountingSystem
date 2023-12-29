using AccountingRooms.Dto;
using AccountingRooms.Model;
using AccountingRooms.Repository;

using Newtonsoft.Json;

namespace AccountingRooms.RabbitMQ;

public class RabbitMQHandler
{
    private BuildingRepository _buildingRepository;

    public RabbitMQHandler(BuildingRepository buildingRepository)
    {
        _buildingRepository = buildingRepository;
    }

    public void Process(string content)
    {
        Console.WriteLine("Process in Handler");

        RabbitMQData data = JsonConvert.DeserializeObject<RabbitMQData>(content);

        Building building = new Building 
        {
            Id = data.Building.Id,
            Floors = data.Building.Floors,
            Name = data.Building.Name,
        };

        Console.WriteLine(data.Action);
        Console.WriteLine(building);

        if ((int)data.Action == (int)RabbitMQAction.Update)
        {
            _buildingRepository.TryUpdateBuilding(building);
        }
        else if ((int)data.Action == (int)RabbitMQAction.Create)
        {
            _buildingRepository.AddNewBuilding(building);
            Console.WriteLine("Created");
        }
        else if ((int)data.Action == (int)RabbitMQAction.Delete)
        {
            _buildingRepository.TryRemoveBuilding(building);
        } 
        else
        {
            throw new NotImplementedException();
        }

        Console.WriteLine("Processed in Handler");
    }
}
