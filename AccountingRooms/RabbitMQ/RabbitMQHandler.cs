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
        RabbitMQData data = JsonConvert.DeserializeObject<RabbitMQData>(content);

        Building building = new Building 
        {
            Id = data.Building.Id,
            Floors = data.Building.Floors,
            Name = data.Building.Name,
        };

        if (data.Action == RabbitMQAction.Update)
        {
            _buildingRepository.TryUpdateBuilding(building);
        }
        else if (data.Action == RabbitMQAction.Create)
        {
            _buildingRepository.AddNewBuilding(building);
        }
        else if (data.Action == RabbitMQAction.Delete)
        {
            _buildingRepository.TrySetBuildingInactive(building);
        } 
        else
        {
            throw new NotImplementedException();
        }
    }
}
