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

    public RoomController(RoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
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


    [HttpGet("get-by-name/{name}")]
    public IActionResult GetRoomsByName(string name)
    {
        return Ok(_roomRepository.GetRoomsByName(name));
    }


    [HttpPost("add-new")]
    public IActionResult AddNewRoom([FromBody] RoomDto roomDto)
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

        _roomRepository.AddNewRoom(room);

        return StatusCode(201);
    }


    [HttpPost("add-new-range")]
    public IActionResult AddNewRoom([FromBody] RoomDto[] roomsDto)
    {
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
            Console.WriteLine(room);
            rooms.Add(room);
        }

        _roomRepository.AddNewRooms(rooms.ToArray());

        return StatusCode(201);
    }

    [HttpPost("update")]
    public IActionResult UpdateRoom([FromBody] Room room)
    {
        if (!_roomRepository.TryUpdateRoom(room))
        {
            return BadRequest();
        }

        return Ok();
    }

    [HttpPost("update-range")]
    public IActionResult UpdateRooms([FromBody] Room[] rooms)
    {
        _roomRepository.UpdateExistsRooms(rooms);

        return Ok();
    }

    [HttpPost("remove")]
    public IActionResult RemoveRoom([FromBody] Room room)
    {
        if (!_roomRepository.TryRemoveRoom(room))
        {
            return BadRequest();
        }

        return Ok();
    }

    [HttpPost("remove-range")]
    public IActionResult RemoveRooms([FromBody] Room[] rooms)
    {
        _roomRepository.RemoveExistsRooms(rooms);

        return Ok();
    }
}
