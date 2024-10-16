namespace WhoIsHome.Shared.Exceptions;

public class EnvironmentHelperException(string message, string variableName) : Exception(message)
{
    public string VariableName { get; } = variableName;
}