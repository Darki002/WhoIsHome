using Google.Cloud.Firestore;
using Microsoft.Extensions.DependencyInjection;

namespace WhoIsHome.Infrastructure;

public static class ServiceProviderConfig
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddTransient<FirestoreDbBuilder>();
        return services;
    } 
}