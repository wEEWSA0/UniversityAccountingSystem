using AccountingBuildings.Data;
using AccountingBuildings.Model;

using Microsoft.EntityFrameworkCore;

namespace AccountingBuildings.Repository;

public class BuildingRepository
{
    private ApplicationDbContext _dbContext;

    public BuildingRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public bool IsExistBuilding(long id)
    {
        return TryGetBuilding(id, out Building building);
    }

    // Только для чтения
    public bool TryGetBuilding(long id, out Building building)
    {
        building = _dbContext.Buildings.AsNoTracking().Where(b => b.Id == id).SingleOrDefault();

        return building != null;
    }

    // Только для работы внутри сервисов данной программы
    public Building GetBuildingOrThrow(long id)
    {
        return _dbContext.Buildings.Where(b => b.Id == id).SingleOrDefault()!;
    }

    public List<Building> GetBuildingsWithLimit(int limit)
    {
        return _dbContext.Buildings.AsNoTracking().OrderBy(o => o.Id).Take(limit).ToList();
    }

    public List<Building> GetBuildingsByName(string name)
    {
        return _dbContext.Buildings.AsNoTracking()
            .Where(b => b.Name.Contains(name))
            .ToList();
    }

    public List<Building> GetBuildingsByNameWithLimit(string name, int limit)
    {
        return _dbContext.Buildings.AsNoTracking()
            .Where(b => b.Name.Contains(name))
            .OrderBy(o => o.Id)
            .Take(limit).ToList();
    }

    public void AddNewBuilding(Building building)
    {
        _dbContext.Buildings.Add(building);
        _dbContext.SaveChanges();
    }

    public void UpdateBuilding(Building building)
    {
        _dbContext.Buildings.Update(building);
        _dbContext.SaveChanges();
    }

    public void RemoveBuilding(Building building)
    {
        _dbContext.Buildings.Remove(building);
        _dbContext.SaveChanges();
    }
}
