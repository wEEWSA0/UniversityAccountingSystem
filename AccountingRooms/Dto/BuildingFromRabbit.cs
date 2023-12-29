using System.ComponentModel.DataAnnotations;

namespace AccountingRooms.Dto;

public class BuildingFromRabbit
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public int Floors { get; set; }
}
