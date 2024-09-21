using WhoIsHome.DataAccess;

namespace WhoIsHome.Host.SetUp;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwagger();
        services.AddControllers();
        
        services.AddWhoIsHomeServices(configuration);
        services.AddDataAccessServices();
        return services;
    }
}