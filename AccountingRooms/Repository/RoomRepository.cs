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

    // Только для работы внутри сервисов данной программы
    public Room GetRoomOrThrow(long id)
    {
        return _dbContext.Rooms.Where(b => b.Id == id).SingleOrDefault()!;
    }

    public List<Room> GetRoomsWithLimit(int limit)
    {
        return _dbContext.Rooms.AsNoTracking().OrderBy(o => o.Id).Take(limit).ToList();
    }

    public void AddNewRoom(Room room)
    {
        _dbContext.Rooms.Add(room);
        _dbContext.SaveChanges();
    }

    public void UpdateRoom(Room room)
    {
        _dbContext.Rooms.Update(room);
        _dbContext.SaveChanges();
    }

    public void RemoveRoom(Room room)
    {
        _dbContext.Rooms.Remove(room);
        _dbContext.SaveChanges();
    }
}
