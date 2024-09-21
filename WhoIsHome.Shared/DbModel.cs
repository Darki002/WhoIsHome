using System.ComponentModel.DataAnnotations;

namespace WhoIsHome.Shared;

public abstract class DbModel
{
    [Key]
    public int Id { get; set; }
    
    public TAggregate ToAggregate<TAggregate>() where TAggregate : AggregateBase
    {
        var propertyInfos = typeof(DbModel).GetProperties();
        return Mapper.Map<TAggregate>(GetType(), this, propertyInfos);
    }
}