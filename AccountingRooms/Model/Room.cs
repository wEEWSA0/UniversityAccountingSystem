using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations;

namespace AccountingRooms.Model;

public class Room
{
    [Key]
    public long Id { get; set; }
    public long BuildingId { get; set; }
    public string Name { get; set; } = null!;
    public RoomType RoomType { get; set; }
    public int Capacity { get; set; }
    public short Floor { get; set; }
    public int Number { get; set; }
}
