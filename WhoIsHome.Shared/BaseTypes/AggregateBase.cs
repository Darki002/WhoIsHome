using WhoIsHome.Shared.Framework;

namespace WhoIsHome.Shared.BaseTypes;

public abstract class AggregateBase
{
    public TDbModel ToDbModel<TDbModel>() where TDbModel : DbModel
    {
        var propertyInfos = typeof(TDbModel).GetProperties();
        return Mapper.Map<TDbModel>(GetType(), this, propertyInfos);
    }
}