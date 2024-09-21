using System.Reflection;

namespace WhoIsHome.Shared;

internal static class Mapper
{
    public static List<TDbModel> ToDbModelList<TDbModel>(this List<Aggregate> models) where TDbModel : DbModel
    {
        return models.Select(m => m.ToDbModel<TDbModel>()).ToList();
    }
    
    public static List<TModel> ToModelList<TModel>(this List<DbModel> models) where TModel : Aggregate
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