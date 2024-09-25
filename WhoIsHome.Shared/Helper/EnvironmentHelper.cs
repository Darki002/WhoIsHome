using WhoIsHome.Shared.Exceptions;

namespace WhoIsHome.Shared.Helper;

public static class EnvironmentHelper
{
    public static string GetVariable(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);
        if (value is null)
        {
            throw new EnvironmentHelperException("Environment variable not found.", name);
        }

        return value;
    }
}