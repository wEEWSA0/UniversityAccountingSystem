﻿using AccountingBuildings.Data;
using AccountingBuildings.Model;

using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;

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

    // todo RemoveExistsBuildings, но поиск по id
    public void RemoveExistsBuildings(Building[] buildings) // todo стоит сообщать, что определенные сущности не удалены
    {
        var buildingsFound = _dbContext.Buildings
            .Where(b => buildings.Contains(b))
            .ToList();

        _dbContext.Buildings.RemoveRange(buildingsFound);
        _dbContext.SaveChanges();
    }

    //public void RemoveExistsBuildingsById(long[] buildingsIds)
    //{
    //    var entities = _dbContext.Buildings
    //        .Where(b => buildingsIds.Contains(b.Id))
    //        .ToList();

    //    _dbContext.Buildings.RemoveRange(entities);
    //    _dbContext.SaveChanges();
    //}
}






































// Это надо разобрать конкретнее (слишком полезно, чтобы не погрузится во все тонкости)
// Conflicts:
// 1. Include
// 2. Fields (like Id, Name)

// Для Id можно сделать IEntityWithId
/*
public class RepositorySetup<T> where T : class
{
    private DbContext _dbContext;
    private DbSet<T> _dbSet;

    public RepositorySetup(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public virtual void RemoveExists(T[] entities)
    {
        var entitiesFound = _dbSet
            .Where(e => entities.Contains(e))
            .Include(x => x.)
            .ToList();

        _dbSet.RemoveRange(entitiesFound);
        _dbContext.SaveChanges();
    }
}*/
