using Microsoft.Extensions.Configuration;
using WhoIsHome.Shared.Exceptions;

namespace WhoIsHome.Shared.Helper;

public static class EnvVariablesConfigExtension
{
    /// <summary>
    /// Key used for the JWT Authentication
    /// </summary>
    public static string GetJwtSecretKey(this IConfiguration config) => config.GetFrom("JWT_SECRET_KEY");

    /// <summary>
    /// API Key used by the middleware in every request to Authorized.
    /// </summary>
    public static string GetApiKey(this IConfiguration config) => config.GetFrom("API_KEY");

    /// <summary>
    /// The SQL Server.
    /// </summary>
    public static string GetMySqlServer(this IConfiguration config) => config.GetFrom("MYSQL_SERVER");
    
    /// <summary>
    /// Port for the Database.
    /// </summary>
    public static string GetMySqlPort(this IConfiguration config) => config.GetFrom("MYSQL_PORT");
    
    /// <summary>
    /// Database that will be used by the Application.
    /// </summary>
    public static string GetMySqlDatabase(this IConfiguration config) => config.GetFrom("MYSQL_DATABASE");
    
    /// <summary>
    /// The User that is being used by the app to connect to the db.
    /// </summary>
    public static string GetMySqlUser(this IConfiguration config) => config.GetFrom("MYSQL_USER");
    
    /// <summary>
    /// The Password for the user that is being used by the app to connect to the db.
    /// </summary>
    public static string GetMySqlPassword(this IConfiguration config) => config.GetFrom("MYSQL_PASSWORD");

    private static string GetFrom(this IConfiguration config, string name)
    {
        return config[name] ?? throw new EnvironmentHelperException("Missing Config", name);
    }
}