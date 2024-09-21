namespace WhoIsHome.Shared;

public abstract class DbModel
{
    public TModel ToModel<TModel>() where TModel : Model
    {
        var propertyInfos = typeof(DbModel).GetProperties();
        return Mapper.Map<TModel>(GetType(), this, propertyInfos);
    }
}