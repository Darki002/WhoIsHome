using System.Reflection;

namespace WhoIsHome.Shared;

public static class Mapper
{
    public static List<TDbModel> ToDbModelList<TDbModel>(this IEnumerable<Aggregate> models) 
        where TDbModel : DbModel
    {
        return models.Select(m => m.ToDbModel<TDbModel>()).ToList();
    }
    
    public static List<TModel> ToModelList<TModel, TDbModel>(this IEnumerable<TDbModel> models) 
        where TModel : Aggregate 
        where TDbModel : DbModel
    {
        return models.Select(m => m.ToModel<TModel>()).ToList();
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