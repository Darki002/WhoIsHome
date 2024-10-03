using System.IO;
using System.Linq;

namespace WhoIsHome.MapperGenerator;

public static class Program
{
    private const string BasePath = "WhoIsHome/Aggregates/Mappers";
    
    public static void Main(string[] args)
    {
        var root = args.First();
        var path = Path.Combine(root, BasePath);
        MappingSourceGenerator.Execute(path);
    }
}