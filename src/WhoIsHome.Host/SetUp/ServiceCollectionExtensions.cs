using WhoIsHome.Host.Authentication;
using WhoIsHome.Shared;
using WhoIsHome.Shared.Authentication;
using WhoIsHome.WebApi;

namespace WhoIsHome.Host.SetUp;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwagger();
        services.AddControllers();

        services.AddHttpContextAccessor();
        services.AddScoped<IUserContextProvider, UserContextProvider>();
        
        services.AddWhoIsHomeServices(configuration)
            .AddWebApiServices()
            .AddWhoIsHomeShared();
        
        return services;
    }
}