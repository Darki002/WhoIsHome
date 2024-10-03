using System.Reflection;

namespace WhoIsHome.Aggregates.Mappers;

// Workaround so that all project are loaded into the Domain
public static class AssemblyLoader
{
    public static Assembly[] GetAssemblies()
    {
        return AppDomain.CurrentDomain.GetAssemblies();
    }
}