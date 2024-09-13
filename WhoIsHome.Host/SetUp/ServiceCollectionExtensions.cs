namespace WhoIsHome.Host.SetUp;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();
        });
        services.AddControllers();
        
        services.AddWhoIsHomeServices(configuration);
        return services;
    }
}