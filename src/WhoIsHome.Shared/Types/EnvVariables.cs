namespace WhoIsHome.Shared.Types;

public static class EnvVariables
{
    /// <summary>
    /// Key used for the JWT Authentication
    /// </summary>
    public const string JwtSecretKey = "JWT_SECRET_KEY";

    /// <summary>
    /// API Key used by the middleware in every request to Authorized.
    /// </summary>
    public const string ApiKey = "API_KEY";

    /// <summary>
    /// The SQL Server.
    /// </summary>
    public const string MySqlServer = "MYSQL_SERVER";
    
    /// <summary>
    /// Port for the Database.
    /// </summary>
    public const string MySqlPort = "MYSQL_PORT";
    
    /// <summary>
    /// Database that will be used by the Application.
    /// </summary>
    public const string MySqlDatabase = "MYSQL_DATABASE";
    
    /// <summary>
    /// The User that is being used by the app to connect to the db.
    /// </summary>
    public const string MySqlUser = "MYSQL_USER";
    
    /// <summary>
    /// The Password for the user that is being used by the app to connect to the db.
    /// </summary>
    public const string MySqlPassword = "MYSQL_PASSWORD";
}