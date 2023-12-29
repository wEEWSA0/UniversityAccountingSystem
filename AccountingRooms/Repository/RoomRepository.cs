using AccountingRooms.Data;
using AccountingRooms.Model;

using Microsoft.EntityFrameworkCore;

namespace AccountingRooms.Repository;

public class RoomRepository
{
    private ApplicationDbContext _dbContext;

    public RoomRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public bool IsExistRoom(long id)
    {
        return TryGetRoom(id, out Room room);
    }

    // Только для чтения
    public bool TryGetRoom(long id, out Room room)
    {
        room = _dbContext.Rooms.AsNoTracking().Where(b => b.Id == id).SingleOrDefault();

        return room != null;
    }

    public List<Room> GetRoomsWithLimit(int limit)
    {
        return _dbContext.Rooms.AsNoTracking().OrderBy(o => o.Id).Take(limit).ToList();
    }

    public List<Room> GetRoomsByName(string name)
    {
        return _dbContext.Rooms.AsNoTracking()
            .Where(b => b.Name.Contains(name))
            .ToList();
    }

    public List<Room> GetRoomsByNameWithLimit(string name, int limit)
    {
        return _dbContext.Rooms.AsNoTracking()
            .Where(b => b.Name.Contains(name))
            .OrderBy(o => o.Id)
            .Take(limit).ToList();
    }

    public void AddNewRoom(Room room)
    {
        _dbContext.Rooms.Add(room);
        _dbContext.SaveChanges();
    }

    public void AddNewRooms(Room[] rooms)
    {
        _dbContext.Rooms.AddRange(rooms);
        _dbContext.SaveChanges();
    }

    public bool TryUpdateRoom(Room room)
    {
        if (IsExistRoom(room.Id))
        {
            return false;
        }

        _dbContext.Rooms.Update(room);
        _dbContext.SaveChanges();

        return true;
    }

    public void UpdateExistsRooms(Room[] rooms)
    {
        var roomsFound = _dbContext.Rooms
            .Where(b => rooms.Contains(b))
            .ToList();

        _dbContext.Rooms.UpdateRange(roomsFound);
        _dbContext.SaveChanges();
    }

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
        var RoomsFound = _dbContext.Rooms
            .Where(b => rooms.Contains(b))
            .ToList();

        _dbContext.Rooms.RemoveRange(RoomsFound);
        _dbContext.SaveChanges();
    }
}
