using Microsoft.Extensions.Configuration;

namespace WhoIsHome.Shared.Configurations;

public class MySqlConfigSection(IConfigurationSection section)
{
    /// <summary>
    /// Key: MYSQL__SERVER
    /// The SQL Server.
    /// </summary>
    public string Server => section.GetString("SERVER");
    
    /// <summary>
    /// Key: MYSQL__PORT
    /// Port for the Database.
    /// </summary>
    public string Port => section.GetString("PORT");
    
    /// <summary>
    /// Key: MYSQL__DATABASE
    /// Database that will be used by the Application.
    /// </summary>
    public string Database => section.GetString("DATABASE");
    
    /// <summary>
    /// Key: MYSQL__USER
    /// The User that is being used by the app to connect to the db.
    /// </summary>
    public string User => section.GetString("USER");
    
    /// <summary>
    /// Key: MYSQL__PASSWORD
    /// The Password for the user that is being used by the app to connect to the db.
    /// </summary>
    public string Password => section.GetString("PASSWORD");
}