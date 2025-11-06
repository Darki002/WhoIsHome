using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using WhoIsHome.Aggregates;
using WhoIsHome.Entities;

namespace WhoIsHome.WebApi;

public static class ServiceProviderConfig
{
    public static IServiceCollection AddWebApiServices(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        return services;
    }
}