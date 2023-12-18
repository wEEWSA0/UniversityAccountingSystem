using AccountingBuildings.Dto;
using AccountingBuildings.Model;
using AccountingBuildings.Repository;

using Microsoft.AspNetCore.Mvc;

namespace AccountingBuildings.Controller;

[ApiController]
[Route("[controller]/")]
public class BuildingController : ControllerBase
{
    private BuildingRepository _buildingRepository;

    public BuildingController(BuildingRepository buildingRepository)
    {
        _buildingRepository = buildingRepository;
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

        return Ok();
    }
}
