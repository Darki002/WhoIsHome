using Microsoft.Extensions.Configuration;
using WhoIsHome.Shared.Exceptions;

namespace WhoIsHome.Shared.Configurations;

public static class EnvVariablesConfigExtension
{
    /// <summary>
    /// Key: JWT_SECRET_KEY
    /// Key used for the JWT Authentication
    /// </summary>
    public static string GetJwtSecretKey(this IConfiguration config) => config.GetString("JWT_SECRET_KEY");

    /// <summary>
    /// Key: API_KEY
    /// API Key used by the middleware in every request to Authorized.
    /// </summary>
    public static string GetApiKey(this IConfiguration config) => config.GetString("API_KEY");

    /// <summary>
    /// MySql Config Section
    /// </summary>
    public static MySqlConfigSection GetMySql(this IConfiguration config) => new(config.GetSection("MYSQL"));

    /// <summary>
    /// Key: PUSH_UP_ENABLED
    /// Enable or Disable sending of Push Up Notifications
    /// </summary>
    public static bool GetPushNotificationEnabled(this IConfiguration config) => config.GetString("PUSH_UP_ENABLED").Equals("true", StringComparison.OrdinalIgnoreCase);
    
    internal static string GetString(this IConfiguration config, string name)
    {
        return config[name] ?? throw new EnvironmentHelperException($"Missing Config for key {name}", name);
    }

    internal static string GetStringOrDefault(this IConfiguration config, string name, string defaultValue)  
    {
        return config[name] ?? defaultValue;
    }
}