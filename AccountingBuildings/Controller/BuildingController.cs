using AccountingBuildings.Dto;
using AccountingBuildings.Model;
using AccountingBuildings.RabbitMQ;
using AccountingBuildings.Repository;

using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;

namespace AccountingBuildings.Controller;

[ApiController]
[Route("[controller]/")]
public class BuildingController : ControllerBase
{
    private BuildingRepository _buildingRepository;
    private IRabbitMQService _rabbitMQService;

    public BuildingController(BuildingRepository buildingRepository, IRabbitMQService rabbitMQService)
    {
        _buildingRepository = buildingRepository;
        _rabbitMQService = rabbitMQService;
    }

    [HttpGet("get/{id:long}")]
    public IActionResult GetBuildingById(long id)
    {
        var result = _buildingRepository.TryGetBuilding(id, out Building building);

        if (result)
        {
            return Ok(building);
        }
        else
        {
            return NotFound();
        }
    }


    [HttpGet("get-by-name/{name}")]
    public IActionResult GetBuildingsByName(string name)
    {
        return Ok(_buildingRepository.GetBuildingsByName(name));
    }


    [HttpGet("get-all-with-limit/{limit:int}")]
    public IActionResult GetBuildingsWithLimit(int limit)
    {
        if (limit <= 0)
        {
            return BadRequest("Limit can't be less than 0");
        }

        return Ok(_buildingRepository.GetBuildingsWithLimit(limit));
    }


    [HttpPost("add-new")]
    public IActionResult AddNewBuilding([FromBody] BuildingDto buildingDto)
    {
        var building = new Building { 
            Name = buildingDto.Name,
            Floors = buildingDto.Floors,
            Address = buildingDto.Address
        };

        _buildingRepository.AddNewBuilding(building);

        RabbitMQData data = new RabbitMQData { Building = building, Action = RabbitMQAction.Create };
        _rabbitMQService.SendMessage(data);

        return StatusCode(201, building.Id);
    }


    [HttpPost("add-new-range")]
    public IActionResult AddNewBuildings([FromBody] BuildingDto[] buildingsDto)
    {
        List<Building> buildings = new();

        foreach (BuildingDto buildingDto in buildingsDto)
        {
            var building = new Building
            {
                Name = buildingDto.Name,
                Floors = buildingDto.Floors,
                Address = buildingDto.Address
            };
            buildings.Add(building);
        }

        _buildingRepository.AddNewBuildings(buildings.ToArray());

        foreach (Building building in buildings)
        {
            RabbitMQData data = new RabbitMQData { Building = building, Action = RabbitMQAction.Create };
            _rabbitMQService.SendMessage(data);
        }

        var buildingsIds = buildings.Select(b => b.Id);

        return StatusCode(201, buildingsIds);
    }


    [HttpPost("update")]
    public IActionResult UpdateBuilding([FromBody] Building building)
    {
        if (!_buildingRepository.TryUpdateBuilding(building))
        {
            return NotFound();
        }

        RabbitMQData data = new RabbitMQData { Building = building, Action = RabbitMQAction.Update };
        _rabbitMQService.SendMessage(data);

        return Ok();
    }


    [HttpPost("update-range")]
    public IActionResult UpdateBuildings([FromBody] Building[] buildings)
    {
        if (!_buildingRepository.TryUpdateBuildings(buildings))
        {
            return BadRequest();
        }

        foreach (Building building in buildings)
        {
            RabbitMQData data = new RabbitMQData { Building = building, Action = RabbitMQAction.Update };
            _rabbitMQService.SendMessage(data);
        }

        return Ok();
    }


    [HttpPost("remove")]
    public IActionResult RemoveBuilding([FromBody] long buildingId)
    {
        if (!_buildingRepository.TrySetBuildingInactive(buildingId))
        {
            return NotFound();
        }

        var building = _buildingRepository.GetBuildingWithAnyActivity(buildingId);

        if (building is null)
        {
            Console.WriteLine("Unexpected value. Building is null");

            return StatusCode(500, "Internal conflict");
        }

        RabbitMQData data = new RabbitMQData { Building = building, Action = RabbitMQAction.Delete };
        _rabbitMQService.SendMessage(data);

        return Ok();
    }


    [HttpPost("remove-range")]
    public IActionResult RemoveBuildings([FromBody] long[] buildingsIds)
    {
        _buildingRepository.SetInactiveExistsBuildings(buildingsIds);

        var buildings = _buildingRepository.GetBuildingsWithAnyActivity(buildingsIds);

        if (!buildings.Any())
        {
            Console.WriteLine("Unexpected value. Buildings is empty");

            return StatusCode(500, "Internal conflict");
        }

        foreach (Building building in buildings)
        {
            RabbitMQData data = new RabbitMQData { Building = building, Action = RabbitMQAction.Delete };
            _rabbitMQService.SendMessage(data);
        }

        return Ok();
    }
}
