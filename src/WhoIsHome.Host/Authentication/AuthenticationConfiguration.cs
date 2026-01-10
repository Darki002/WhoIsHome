using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using WhoIsHome.Shared.Configurations;

namespace WhoIsHome.Host.Authentication;

public static class AuthenticationConfiguration
{
    public static IServiceCollection AddWihAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = configuration.GetJwtSecretKey();
        SetUpJwt(services, jwtSettings, secretKey);

        services.AddScoped<ApiKeyMiddleware>();

        return services;
    }

    public static void UseWihAuthentication(this WebApplication app)
    {
        app.UseMiddleware<ApiKeyMiddleware>();
    }

    private static void SetUpJwt(IServiceCollection services, IConfiguration jwtSettings, string secretKey)
    {
        var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secretKey));

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true, // Keep this to validate the token's Issuer
                    ValidIssuer = jwtSettings["Issuer"], // Set your valid Issuer here

                    ValidateAudience = true, // Keep this to validate the Audience
                    ValidAudience = jwtSettings["Audience"], // Set your valid Audience here

                    ValidateLifetime = true, // Ensure the token has not expired
                    ValidateIssuerSigningKey = true, // Ensure the signing key is valid and correctly configured

                    IssuerSigningKey = new SymmetricSecurityKey(hmac.Key) // The key used for signing the token
                };

                // Enable token validation errors to be logged
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogInformation("Authentication failed: {Message}", context.Exception.Message);
                        return Task.CompletedTask;
                    }
                };
            });
    }
}