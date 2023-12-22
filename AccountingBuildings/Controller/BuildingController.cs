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
            return BadRequest("Not found");
        }
    }

    [HttpGet("get-by-name/{name:string}")]
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

        _rabbitMQService.SendMessage("Added new entity with id: " + building.Id);

        return StatusCode(201);
    }

    [HttpPost("update")]
    public IActionResult UpdateBuilding([FromBody] Building building)
    {
        if (!_buildingRepository.IsExistBuilding(building.Id))
        {
            return BadRequest();
        }

        _buildingRepository.UpdateBuilding(building);

        _rabbitMQService.SendMessage(building);

        return Ok();
    }

    [HttpPost("remove")]
    public IActionResult RemoveBuilding([FromBody] Building building)
    {
        if (!_buildingRepository.IsExistBuilding(building.Id))
        {
            return BadRequest();
        }

        _buildingRepository.RemoveBuilding(building);

        _rabbitMQService.SendMessage("Removed entity with id: " + building.Id);

        return Ok();
    }
}
