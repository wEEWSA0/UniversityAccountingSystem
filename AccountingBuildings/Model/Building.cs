using System.ComponentModel.DataAnnotations;

namespace AccountingBuildings.Model;

public class Building
{
    [Key]
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public int Floors { get; set; }
    public bool IsActive { get; set; } = true;
}
