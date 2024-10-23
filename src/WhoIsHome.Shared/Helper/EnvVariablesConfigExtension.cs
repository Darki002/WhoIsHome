using Microsoft.Extensions.Configuration;
using WhoIsHome.Shared.Exceptions;

namespace WhoIsHome.Shared.Helper;

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
    /// <returns></returns>
    public static IConfigurationSection GetMySql(this IConfiguration config) => config.GetSection("MYSQL");
    
    /// <summary>
    /// Key: MYSQL__SERVER
    /// The SQL Server.
    /// </summary>
    public static string GetMySqlServer(this IConfigurationSection config) => config.GetString("MYSQL:SERVER");
    
    /// <summary>
    /// Key: MYSQL__PORT
    /// Port for the Database.
    /// </summary>
    public static string GetMySqlPort(this IConfigurationSection config) => config.GetString("MYSQL:PORT");
    
    /// <summary>
    /// Key: MYSQL__DATABASE
    /// Database that will be used by the Application.
    /// </summary>
    public static string GetMySqlDatabase(this IConfigurationSection config) => config.GetString("MYSQL:DATABASE");
    
    /// <summary>
    /// Key: MYSQL__USER
    /// The User that is being used by the app to connect to the db.
    /// </summary>
    public static string GetMySqlUser(this IConfigurationSection config) => config.GetString("MYSQL:USER");
    
    /// <summary>
    /// Key: MYSQL__PASSWORD
    /// The Password for the user that is being used by the app to connect to the db.
    /// </summary>
    public static string GetMySqlPassword(this IConfigurationSection config) => config.GetString("MYSQL:PASSWORD");

    private static IConfigurationSection GetSection(this IConfiguration config, string name)
    {
        return config.GetSection(name) ?? throw new EnvironmentHelperException("Missing Config Section", name);
    }
    
    private static string GetString(this IConfiguration config, string name)
    {
        return config[name] ?? throw new EnvironmentHelperException("Missing Config", name);
    }
}