using Microsoft.Extensions.Configuration;

namespace WhoIsHome.Shared.Configurations;

public class DbConnectionInfo(IConfigurationSection section)
{
    /// <summary>
    /// Key: MYSQL__SERVER
    /// The SQL Server.
    /// </summary>
    public string Host => section.GetString("HOST");
    
    /// <summary>
    /// Key: MYSQL__PORT (optional)
    /// Port for the Database.
    /// </summary>
    public string Port => section.GetStringOrDefault("PORT", "3306");
    
    /// <summary>
    /// Key: MYSQL__DATABASE (optional)
    /// Database that will be used by the Application.
    /// </summary>
    public string Database => section.GetStringOrDefault("DATABASE", "WohIsHome");
    
    /// <summary>
    /// Key: MYSQL__USER
    /// The User that is being used by the app to connect to the db.
    /// </summary>
    public string User => section.GetStringOrDefault("USER", "root");
    
    /// <summary>
    /// Key: MYSQL__PASSWORD
    /// The Password for the user that is being used by the app to connect to the db.
    /// </summary>
    public string Password => section.GetString("PASSWORD");
}