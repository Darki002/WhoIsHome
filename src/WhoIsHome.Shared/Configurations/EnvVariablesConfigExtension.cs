using Microsoft.Extensions.Configuration;

namespace WhoIsHome.Shared.Configurations;

public static class EnvVariablesConfigExtension
{
    /// <summary>
    /// Key: JWT_SECRET_KEY
    /// Key used for the JWT Authentication
    /// </summary>
    public static string GetJwtSecretKey(this IConfiguration config)
    {
        return config.GetString("JWT_SECRET_KEY");
    }

    /// <summary>
    /// Key: API_KEY
    /// API Key used by the middleware in every request to Authorized.
    /// </summary>
    public static string GetApiKey(this IConfiguration config)
    {
        return config.GetString("API_KEY");
    }

    /// <summary>
    /// MySql Config Section
    /// </summary>
    public static DbConnectionInfo GetDbConnectionInfo(this IConfiguration config) => new(config.GetSection("DB"));

    /// <summary>
    /// Key: PUSH_UP_ENABLED
    /// Enable or Disable sending of Push Up Notifications
    /// </summary>
    public static bool GetPushNotificationEnabled(this IConfiguration config)
    {
        return config.GetStringOrDefault("PUSH_UP_ENABLED", "false")
            .Equals("true", StringComparison.OrdinalIgnoreCase);
    }

    internal static string GetString(this IConfiguration config, string name)
    {
        return config[name] ?? throw new ArgumentException($"Missing Config for key {name}", name);
    }

    internal static string GetStringOrDefault(this IConfiguration config, string name, string defaultValue)
    {
        return config[name] ?? defaultValue;
    }
}