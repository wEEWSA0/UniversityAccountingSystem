using AccountingRooms.Data;
using AccountingRooms.Model;

using Microsoft.EntityFrameworkCore;

namespace AccountingRooms.Repository;

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

    public bool TryGetBuilding(long id, out Building building)
    {
        building = _dbContext.Buildings.AsNoTracking().Where(b => b.Id == id).SingleOrDefault();

        return building != null;
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

    public void AddNewBuildings(Building[] buildings)
    {
        _dbContext.Buildings.AddRange(buildings);
        _dbContext.SaveChanges();
    }

    public bool TryUpdateBuilding(Building building)
    {
        if (IsExistBuilding(building.Id))
        {
            return false;
        }

        _dbContext.Buildings.Update(building);
        _dbContext.SaveChanges();

        return true;
    }

    public void UpdateExistsBuildings(Building[] buildings)
    {
        var buildingsFound = _dbContext.Buildings
            .Where(b => buildings.Contains(b))
            .ToList();

        _dbContext.Buildings.UpdateRange(buildingsFound);
        _dbContext.SaveChanges();
    }

    public void RemoveBuildingOrThrow(Building building)
    {
        _dbContext.Buildings.Remove(building);
        _dbContext.SaveChanges();
    }

    public bool TryRemoveBuilding(Building building)
    {
        if (IsExistBuilding(building.Id))
        {
            return false;
        }

        _dbContext.Buildings.Remove(building);
        _dbContext.SaveChanges();

        return true;
    }

    public void RemoveExistsBuildings(Building[] buildings)
    {
        var buildingsFound = _dbContext.Buildings
            .Where(b => buildings.Contains(b))
            .ToList();

        _dbContext.Buildings.RemoveRange(buildingsFound);
        _dbContext.SaveChanges();
    }
}
