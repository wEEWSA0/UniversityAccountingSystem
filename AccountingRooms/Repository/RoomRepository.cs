using AccountingRooms.Data;
using AccountingRooms.Model;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountingRooms.Repository;

public class RoomRepository
{
    private ApplicationDbContext _dbContext;

    private IQueryable<Room> _activeRooms;

    public RoomRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;

        _activeRooms = _dbContext.Rooms.Where(b => b.IsActive);
    }

    public bool IsExistRoom(long id)
    {
        return TryGetRoom(id, out Room room);
    }

    public bool IsExistRoomByNumberAndBuildingId(int number, long buildingId)
    {
        return _activeRooms
            .AsNoTracking()
            .Where(r => r.Number == number && r.BuildingId == buildingId)
            .Count() == 1;
    }

    public bool IsExistRoomsByNumbersAndBuildingsIds(int[] numbers, long[] buildingsIds)
    {
        if (numbers.Length !=  buildingsIds.Length)
        {
            throw new ArgumentException("Numbers length should equals buildingsIds length");
        }

        return _activeRooms
            .AsNoTracking()
            .Where(r => numbers.Contains(r.Number) && buildingsIds.Contains(r.BuildingId))
            .Count() == numbers.Length;
    }

    public bool TryGetRoom(long id, out Room room)
    {
        room = _activeRooms.AsNoTracking()
            .Where(b => b.IsActive)
            .Where(b => b.Id == id)
            .Include(b => b.Building)
            .SingleOrDefault();

        return room != null;
    }

    public List<Room> GetRoomsWithLimit(int limit)
    {
        return _activeRooms.AsNoTracking()
            .OrderBy(o => o.Id)
            .Take(limit)
            .Include(b => b.Building)
            .ToList();
    }

    public List<Room> GetRoomsByName(string name)
    {
        return _activeRooms.AsNoTracking()
            .Where(b => b.Name.Contains(name))
            .Include(b => b.Building)
            .ToList();
    }

    public List<Room> GetRoomsByNameWithLimit(string name, int limit)
    {
        return _activeRooms.AsNoTracking()
            .Where(b => b.Name.Contains(name))
            .OrderBy(o => o.Id)
            .Take(limit)
            .Include(b => b.Building)
            .ToList();
    }

    public bool TryAddNewRoom(Room room)
    {
        if (IsExistRoomByNumberAndBuildingId(room.Number, room.BuildingId))
        {
            return false;
        }

        _dbContext.Rooms.Add(room);
        _dbContext.SaveChanges();

        return true;
    }

    public bool TryAddNewRooms(Room[] rooms)
    {
        var param = (
            rooms.Select(r => r.Number).ToArray(),
            rooms.Select(r => r.BuildingId).ToArray()
            );

        if (IsExistRoomsByNumbersAndBuildingsIds(param.Item1, param.Item2))
        {
            return false;
        }

        _dbContext.Rooms.AddRange(rooms);
        _dbContext.SaveChanges();

        return true;
    }

    public bool TryUpdateRoom(Room room)
    {
        if (!TryGetRoom(room.Id, out var oldRoom))
        {
            return false;
        }

        if (oldRoom.Number != room.Number || oldRoom.BuildingId != room.BuildingId)
        {
            if (IsExistRoomByNumberAndBuildingId(room.Number, room.BuildingId))
            {
                return false;
            }
        }

        _dbContext.Rooms.Update(room);
        _dbContext.SaveChanges();

        return true;
    }

    public bool TryUpdateRooms(Room[] rooms)
    {
        var roomsIds = rooms.Select(r => r.Id).ToList();

        var roomsFoundIds = _activeRooms
            .Select(r => r.Id)
            .Where(id => roomsIds.Contains(id))
            .ToList();
        
        if (roomsFoundIds.Count != roomsIds.Count)
        {
            return false;
        }

        _dbContext.Rooms.UpdateRange(rooms);
        _dbContext.SaveChanges();

        return true;
    }
    /*
    public void RemoveRoomOrThrow(Room room)
    {
        _dbContext.Rooms.Remove(room);
        _dbContext.SaveChanges();
    }

    public bool TryRemoveRoom(Room room)
    {
        if (IsExistRoom(room.Id))
        {
            return false;
        }

        _dbContext.Rooms.Remove(room);
        _dbContext.SaveChanges();

        return true;
    }

    public void RemoveExistsRooms(Room[] rooms)
    {
        var roomsIdsFound = _dbContext.Rooms
            .Where(id => rooms.Contains(id))
            .ToList();

        _dbContext.Rooms.RemoveRange(roomsIdsFound);
        _dbContext.SaveChanges();
    }*/

    public bool TrySetRoomInactive(long roomId)
    {
        if (!TryGetRoom(roomId, out var room))
        {
            return false;
        }

        room.IsActive = false;

        _dbContext.Rooms.Update(room);
        _dbContext.SaveChanges();

        return true;
    }

    public void SetInactiveExistsRooms(long[] roomsIds)
    {
        var roomsFound = _activeRooms
            .Where(r => roomsIds.Contains(r.Id))
            .ToList();

        foreach (Room room in roomsFound)
        {
            room.IsActive = false;
        }

        _dbContext.Rooms.UpdateRange(roomsFound);
        _dbContext.SaveChanges();
    }
}
