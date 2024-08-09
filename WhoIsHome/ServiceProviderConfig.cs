using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using WhoIsHome.QueryHandler.DailyOverview;
using WhoIsHome.QueryHandler.PersonOverview;
using WhoIsHome.Services.Events;
using WhoIsHome.Services.Persons;
using WhoIsHome.Services.RepeatedEvents;

namespace WhoIsHome;

public static class ServiceProviderConfig
{
    public static IServiceCollection AddWhoIsHomeServices(this IServiceCollection services, IConfiguration builderConfiguration)
    {
        var projectId = builderConfiguration["Firebase:Firestore:Project_id"];
        services.AddFirestoreDb(f => f.ProjectId = projectId);

        services.AddTransient<IPersonService, PersonService>();
        services.AddTransient<IEventService, EventService>();
        services.AddTransient<IRepeatedEventService, RepeatedEventService>();

        services.AddTransient<DailyOverviewQueryHandler>();
        services.AddTransient<PersonOverviewQueryHandler>();
        
        return services;
    } 
}