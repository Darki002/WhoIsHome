using Microsoft.OpenApi.Models;
using WhoIsHome.Host.Authentication;

namespace WhoIsHome.Host.SetUp;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();
            
            c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
            {
                Description = "API Key needed to access the endpoints. API Key should be passed as a request header.",
                In = ParameterLocation.Header,
                Name = ApiKeyMiddleware.ApiKeyHeaderName,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "ApiKeyScheme"
            });
            
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "ApiKey"
                        },
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
        });
        return services;
    }
}