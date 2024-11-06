using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WhoIsHome.Host.Authentication;

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

    private static void AddJwt(this SwaggerGenOptions c)
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "JwtAuthApp API", Version = "v1" });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = """
                          JWT Authorization header using the Bearer scheme. 
                                                Example: 'Bearer 12345abcdef'
                                                Note: Is NOT used by the /Auth endpoints.
                          """,
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,
                },
                new List<string>()
            }
        });
    }

    private static void AddApiKey(this SwaggerGenOptions c)
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
    }
}