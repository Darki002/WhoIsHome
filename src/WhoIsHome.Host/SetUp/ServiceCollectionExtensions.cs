using System.Text.Json.Serialization;
using WhoIsHome.Host.Authentication;
using WhoIsHome.Shared;
using WhoIsHome.Shared.Authentication;
using WhoIsHome.Shared.Types;
using WhoIsHome.WebApi;

namespace WhoIsHome.Host.SetUp;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwagger();
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter<PresenceType>());
        });

        services.AddHttpContextAccessor();
        services.AddScoped<IUserContextProvider, UserContextProvider>();
        
        services.AddWhoIsHomeServices(configuration)
            .AddWebApiServices()
            .AddWhoIsHomeShared();
        
        return services;
    }
}