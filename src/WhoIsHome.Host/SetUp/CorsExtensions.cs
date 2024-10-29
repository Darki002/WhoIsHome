namespace WhoIsHome.Host.SetUp;

public static class CorsExtensions
{
    public static IServiceCollection AddCorsPolicy(this IServiceCollection service)
    {
        service.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return service;
    }

    public static void UseCorsPolicy(this WebApplication app)
    {
        app.UseCors("AllowAll");
    }
}