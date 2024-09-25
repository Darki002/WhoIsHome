using System.ComponentModel.DataAnnotations;
using WhoIsHome.Shared.Framework;

namespace WhoIsHome.Shared.BaseTypes;

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