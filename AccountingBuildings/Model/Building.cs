using System.ComponentModel.DataAnnotations;

namespace AccountingBuildings.Model;

public class Building// : EntityWithId<long, int>
{
    [Key]
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public int Floors { get; set; }

}

//TODO move to new Net 8 project
/*
public abstract class EntityWithId<>
{
    // ref работает в более поздной версии
    private T _id;

    public EntityWithId(T id)
    {
        _id = id;
    }
}*/
