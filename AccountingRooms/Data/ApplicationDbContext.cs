using AccountingRooms.Model;

using Microsoft.EntityFrameworkCore;

namespace AccountingRooms.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    protected ApplicationDbContext()
    {
    }

    public DbSet<Room> Rooms { get; set; }
    public DbSet<Building> Buildings { get; set; }
}

