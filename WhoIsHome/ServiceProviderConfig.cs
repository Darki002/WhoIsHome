using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using WhoIsHome.Persons;

namespace WhoIsHome;

public static class ServiceProviderConfig
{
    public static IServiceCollection AddWhoIsHomeServices(this IServiceCollection services, IConfiguration builderConfiguration)
    {
        var projectId = builderConfiguration["Firebase:Firestore:Project_id"];
        services.AddFirestoreDb(f => f.ProjectId = projectId);

        services.AddTransient<IPersonService, PersonService>();
        
        return services;
    } 
}