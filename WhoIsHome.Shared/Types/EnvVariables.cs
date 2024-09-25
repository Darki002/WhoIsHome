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
}