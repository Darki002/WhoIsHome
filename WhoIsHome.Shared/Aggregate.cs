namespace WhoIsHome.Shared;

public abstract class Aggregate
{
    public TDbModel ToDbModel<TDbModel>() where TDbModel : DbModel
    {
        var propertyInfos = typeof(TDbModel).GetProperties();
        return Mapper.Map<TDbModel>(GetType(), this, propertyInfos);
    }
}