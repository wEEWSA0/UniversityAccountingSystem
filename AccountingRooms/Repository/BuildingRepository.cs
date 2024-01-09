using AccountingRooms.Data;
using AccountingRooms.Model;

using Microsoft.EntityFrameworkCore;

namespace AccountingRooms.Repository;

public class BuildingRepository
{
    private ApplicationDbContext _dbContext;

    private IQueryable<Building> _activeBuildings;

    public BuildingRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;

        _activeBuildings = _dbContext.Buildings.Where(b => b.IsActive);
    }

    public bool IsExistBuilding(long id)
    {
        return TryGetBuilding(id, out Building building);
    }

    public bool IsExistsBuildings(long[] ids)
    {
        return _activeBuildings
            .AsNoTracking()
            .Select(b => b.Id)
            .Where(id => ids.Contains(id))
            .Count() == ids.Length;
    }

    public bool TryGetBuilding(long id, out Building building)
    {
        building = _activeBuildings.AsNoTracking().Where(b => b.Id == id).SingleOrDefault();

        return building != null;
    }

    public List<Building> GetBuildingsWithLimit(int limit)
    {
        return _activeBuildings.AsNoTracking().OrderBy(o => o.Id).Take(limit).ToList();
    }

    public List<Building> GetBuildingsByName(string name)
    {
        return _activeBuildings.AsNoTracking()
            .Where(b => b.Name.Contains(name))
            .ToList();
    }

    public List<Building> GetBuildingsByNameWithLimit(string name, int limit)
    {
        return _activeBuildings.AsNoTracking()
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
        if (!IsExistBuilding(building.Id))
        {
            return false;
        }

        _dbContext.Buildings.Update(building);
        _dbContext.SaveChanges();

        return true;
    }

    public bool TryUpdateBuildings(Building[] buildings)
    {
        var buildingsIds = buildings.Select(b => b.Id).ToList();

        var buildingsFoundIds = _activeBuildings
            .Select(b => b.Id)
            .Where(id => buildingsIds.Contains(id))
            .ToList();

        if (buildingsFoundIds.Count != buildingsIds.Count)
        {
            return false;
        }

        _dbContext.Buildings.UpdateRange(buildings);
        _dbContext.SaveChanges();

        return true;
    }
    /*
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
            .Where(id => buildings.Contains(id))
            .ToList();

        _dbContext.Buildings.RemoveRange(buildingsFound);
        _dbContext.SaveChanges();
    }*/

    public bool TrySetBuildingInactive(Building building)
    {
        if (!IsExistBuilding(building.Id))
        {
            return false;
        }

        building.IsActive = false;

        _dbContext.Buildings.Update(building);
        _dbContext.SaveChanges();

        return true;
    }

    public void SetInactiveExistsBuildings(Building[] buildings)
    {
        var buildingsFound = _activeBuildings
            .Where(b => buildings.Contains(b))
            .ToList();

        foreach (Building building in buildingsFound)
        {
            building.IsActive = false;
        }

        _dbContext.Buildings.UpdateRange(buildingsFound);
        _dbContext.SaveChanges();
    }
}
