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
    private BuildingRepository _buildingRepository;

    public RoomController(RoomRepository roomRepository, BuildingRepository buildingRepository)
    {
        _roomRepository = roomRepository;
        _buildingRepository = buildingRepository;
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
            return NotFound("Not found");
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


    [HttpGet("get-by-name/{name}")]
    public IActionResult GetRoomsByName(string name)
    {
        return Ok(_roomRepository.GetRoomsByName(name));
    }


    [HttpPost("add-new")]
    public IActionResult AddNewRoom([FromBody] RoomDto roomDto)
    {
        if (!_buildingRepository.IsExistBuilding(roomDto.BuildingId))
        {
            return NotFound("Building not exists");
        }

        var room = new Room
        {
            Name = roomDto.Name,
            BuildingId = roomDto.BuildingId,
            Capacity = roomDto.Capacity,
            Floor = roomDto.Floor,
            Number = roomDto.Number,
            RoomType = roomDto.RoomType
        };

        if (!_roomRepository.TryAddNewRoom(room))
        {
            return BadRequest("Already exists room by this number and buildingId");
        }

        return StatusCode(201);
    }


    [HttpPost("add-new-range")]
    public IActionResult AddNewRooms([FromBody] RoomDto[] roomsDto)
    {
        var roomsIds = roomsDto
            .Select(r => r.BuildingId)
            .ToArray();

        if (!_buildingRepository.IsExistsBuildings(roomsIds))
        {
            return NotFound("Buildings not exists");
        }

        List<Room> rooms = new();

        foreach (RoomDto roomDto in roomsDto)
        {
            var room = new Room
            {
                Name = roomDto.Name,
                BuildingId = roomDto.BuildingId,
                Capacity = roomDto.Capacity,
                Floor = roomDto.Floor,
                Number = roomDto.Number,
                RoomType = roomDto.RoomType
            };
            rooms.Add(room);
        }

        if (!_roomRepository.TryAddNewRooms(rooms.ToArray()))
        {
            return BadRequest();
        }

        return StatusCode(201);
    }


    [HttpPost("update")]
    public IActionResult UpdateRoom([FromBody] RoomUpdateDto roomDto)
    {
        if (!_buildingRepository.IsExistBuilding(roomDto.BuildingId))
        {
            return NotFound("Building not exists");
        }

        var room = new Room
        {
            Id = roomDto.Id,
            BuildingId = roomDto.BuildingId,
            Capacity = roomDto.Capacity,
            Floor = roomDto.Floor,
            Number = roomDto.Number,
            RoomType = roomDto.RoomType,
            IsActive = roomDto.IsActive,
            Name = roomDto.Name
        };

        if (!_roomRepository.TryUpdateRoom(room))
        {
            return BadRequest();
        }

        return Ok();
    }


    [HttpPost("update-range")]
    public IActionResult UpdateRooms([FromBody] RoomUpdateDto[] roomsDtos)
    {
        var roomsIds = roomsDtos
            .Select(r => r.BuildingId)
            .ToArray();

        if (!_buildingRepository.IsExistsBuildings(roomsIds))
        {
            return NotFound("Buildings not exists");
        }

        List<Room> rooms = new();

        foreach (var roomDto in roomsDtos)
        {
            rooms.Add(new Room
            {
                Id = roomDto.Id,
                BuildingId = roomDto.BuildingId,
                Capacity = roomDto.Capacity,
                Floor = roomDto.Floor,
                Number = roomDto.Number,
                RoomType = roomDto.RoomType,
                IsActive = roomDto.IsActive,
                Name = roomDto.Name
            });
        }

        if (!_roomRepository.TryUpdateRooms(rooms.ToArray()))
        {
            return BadRequest();
        }

        return Ok();
    }


    [HttpPost("remove")]
    public IActionResult RemoveRoom([FromBody] long roomId)
    {
        if (!_roomRepository.TrySetRoomInactive(roomId))
        {
            return NotFound();
        }

        return Ok();
    }


    [HttpPost("remove-range")]
    public IActionResult RemoveRooms([FromBody] long[] roomsIds)
    {
        _roomRepository.SetInactiveExistsRooms(roomsIds);

        return Ok();
    }
}
