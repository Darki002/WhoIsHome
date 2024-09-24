using Microsoft.Extensions.DependencyInjection;
using WhoIsHome.WebApi.UserAuthentication;

namespace WhoIsHome.WebApi;

public static class ServiceProviderConfig
{
    public static IServiceCollection AddWebApiServices(this IServiceCollection services)
    {
        services.AddTransient<JwtTokenService>();
        return services;
    }
}