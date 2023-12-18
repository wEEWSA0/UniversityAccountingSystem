using AccountingRooms.Model;

namespace AccountingRooms.Dto;

public class RoomDto
{
    public long BuildingId { get; set; }
    public string Name { get; set; } = null!;
    public RoomType RoomType { get; set; }
    public int Capacity { get; set; }
    public short Floor { get; set; }
    public int Number { get; set; }
}
