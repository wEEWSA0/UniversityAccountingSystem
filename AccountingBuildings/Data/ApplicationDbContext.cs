using AccountingBuildings.Model;

using Microsoft.EntityFrameworkCore;

namespace AccountingBuildings.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    protected ApplicationDbContext()
    {
    }

    public DbSet<Building> Buildings { get; set; }
}
