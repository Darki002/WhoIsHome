using System.Reflection;

namespace WhoIsHome.Shared;

public static class Mapper
{
    public static List<TDbModel> ToDbModelList<TDbModel, TModel>(this List<TModel> models)
    {
        var propertyInfos = typeof(TDbModel).GetProperties();
        return models.Select(m => Map<TDbModel, TModel>(m, propertyInfos)).ToList();
    }
    
    public static TDbModel ToDbModel<TDbModel, TModel>(this TModel model)
    {
        var propertyInfos = typeof(TDbModel).GetProperties();
        return Map<TDbModel, TModel>(model, propertyInfos);
    }
    
    public static List<TModel> ToModelList<TModel, TDbModel>(this List<TDbModel> models)
    {
        var propertyInfos = typeof(TDbModel).GetProperties();
        return models.Select(m => Map<TModel, TDbModel>(m, propertyInfos)).ToList();
    }
    
    public static TModel ToModel<TModel, TDbModel>(this TDbModel dbModel)
    {
        var propertyInfos = typeof(TDbModel).GetProperties();
        return Map<TModel, TDbModel>(dbModel, propertyInfos);
    }
    
    private static TTo Map<TTo, TFrom>(TFrom model, PropertyInfo[] propertyInfos)
    {
        var dbModel = Activator.CreateInstance<TTo>();
        
        foreach (var propertyInfo in propertyInfos)
        {
            var value = typeof(TFrom).GetProperty(propertyInfo.Name)?.GetValue(model);
            propertyInfo.SetValue(dbModel, value);
        }

        return dbModel;
    }
}