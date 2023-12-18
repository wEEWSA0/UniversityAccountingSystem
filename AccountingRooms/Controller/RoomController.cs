using AccountingRooms.Dto;
using AccountingRooms.Model;
using AccountingRooms.Repository;

using Microsoft.AspNetCore.Mvc;

namespace AccountingRooms.Controller;

[ApiController]
[Route("[controller]/")]
public class RoomController : ControllerBase
{
    private RoomRepository _roomRepository;

    public RoomController(RoomRepository buildingRepository)
    {
        _roomRepository = buildingRepository;
    }

    [HttpGet("get/{id:long}")]
    public IActionResult GetRoomById(long id)
    {
        var result = _roomRepository.TryGetRoom(id, out Room room);

        if (result)
        {
            return Ok(room);
        }
        else
        {
            return BadRequest("Not found");
        }
    }

    [HttpGet("get-all-with-limit/{limit:int}")]
    public IActionResult GetRoomsWithLimit(int limit)
    {
        if (limit <= 0)
        {
            return BadRequest("Limit can't be less than 0");
        }

        return Ok(_roomRepository.GetRoomsWithLimit(limit));
    }

    [HttpPost("add-new")]
    public IActionResult AddNewRoom([FromBody] RoomDto roomDto)
    {
        var building = new Room
        {
            Name = roomDto.Name,
            BuildingId = roomDto.BuildingId,
            Capacity = roomDto.Capacity,
            Floor = roomDto.Floor,
            Number = roomDto.Number,
            RoomType = roomDto.RoomType
        };

        _roomRepository.AddNewRoom(building);

        return StatusCode(201);
    }

    [HttpPost("update")]
    public IActionResult UpdateRoom([FromBody] Room room)
    {
        if (!_roomRepository.IsExistRoom(room.Id))
        {
            return BadRequest();
        }

        _roomRepository.UpdateRoom(room);

        return Ok();
    }

    [HttpPost("remove")]
    public IActionResult RemoveRoom([FromBody] Room room)
    {
        if (!_roomRepository.IsExistRoom(room.Id))
        {
            return BadRequest();
        }

        _roomRepository.RemoveRoom(room);

        return Ok();
    }
}
