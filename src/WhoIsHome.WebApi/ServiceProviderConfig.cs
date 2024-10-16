using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using WhoIsHome.Aggregates;
using WhoIsHome.WebApi.UserAuthentication;

namespace WhoIsHome.WebApi;

public static class ServiceProviderConfig
{
    public static IServiceCollection AddWebApiServices(this IServiceCollection services)
    {
        services.AddTransient<JwtTokenService>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        return services;
    }
}