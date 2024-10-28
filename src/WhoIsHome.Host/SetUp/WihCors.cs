namespace WhoIsHome.Host.SetUp;

public static class WihCors
{
    public static IServiceCollection AddWihCors(this IServiceCollection service)
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
}