using Swashbuckle.AspNetCore.SwaggerGen;
using WhoIsHome.Host.Authentication;
using Microsoft.OpenApi;

namespace WhoIsHome.Host.SetUp;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.AddApiKey();
            c.AddJwt();
        });
        return services;
    }

    private static void AddJwt(this SwaggerGenOptions options)
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "JwtAuthApp API", Version = "v1" });

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = """
                          JWT Authorization header using the Bearer scheme. 
                                                Example: 'Bearer 12345abcdef'
                                                Note: Is NOT used by the /Auth endpoints.
                          """,
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });
        
        options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("bearer", document)] = []
        });
    }

    private static void AddApiKey(this SwaggerGenOptions options)
    {
        options.EnableAnnotations();
            
        options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
        {
            Description = "API Key needed to access the endpoints. API Key should be passed as a request header.",
            In = ParameterLocation.Header,
            Name = ApiKeyMiddleware.ApiKeyHeaderName,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "ApiKeyScheme"
        });
            
        options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("ApiKeyScheme", document)] = []
        });
    }
}