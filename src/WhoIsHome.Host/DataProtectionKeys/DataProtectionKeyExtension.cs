using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using WhoIsHome.Shared.Configurations;

namespace WhoIsHome.Host.DataProtectionKeys;

public static class DataProtectionKeyExtension
{
    public static IServiceCollection AddDataProtectionKey(this IServiceCollection service, IConfiguration configuration)
    {
        var connectionString = BuildConnectionString(configuration);
        service.AddDbContext<DataProtectionKeyContext>(c => c.UseMySQL(connectionString));
        service.AddDataProtection()
            .PersistKeysToDbContext<DataProtectionKeyContext>();
        
        return service;
    }
    
    private static string BuildConnectionString(IConfiguration configuration)
    {
        var mysql = configuration.GetMySql();
        return $"Server={mysql.Server};Port={mysql.Port};Database={mysql.Database};User={mysql.User};Password={mysql.Password};";
    }
}