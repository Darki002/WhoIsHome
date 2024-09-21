using System.Reflection;

namespace WhoIsHome.Shared;

public static class Mapper
{
    public static List<TDbModel> ToDbModelList<TDbModel>(this IEnumerable<AggregateBase> models) 
        where TDbModel : DbModel
    {
        return models.Select(m => m.ToDbModel<TDbModel>()).ToList();
    }
    
    public static List<TAggregate> ToAggregateList<TAggregate, TDbModel>(this IEnumerable<TDbModel> models) 
        where TAggregate : AggregateBase 
        where TDbModel : DbModel
    {
        return models.Select(m => m.ToModel<TAggregate>()).ToList();
    }
    
    internal static TTo Map<TTo>(Type modelType, object model, PropertyInfo[] propertyInfos)
    {
        var dbModel = Activator.CreateInstance<TTo>();
        
        foreach (var propertyInfo in propertyInfos)
        {
            var value = modelType.GetProperty(propertyInfo.Name)?.GetValue(model);
            propertyInfo.SetValue(dbModel, value);
        }

        return dbModel;
    }
}