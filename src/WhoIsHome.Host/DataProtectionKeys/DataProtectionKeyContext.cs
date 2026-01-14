using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WhoIsHome.Host.DataProtectionKeys;

public class DataProtectionKeyContext(DbContextOptions<DataProtectionKeyContext> options) : DbContext(options), 
    IDataProtectionKeyContext
{
    public virtual DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
}

// Used for EF Core Migrations
// ReSharper disable once UnusedType.Global
public class WhoIsHomeContextFactory : IDesignTimeDbContextFactory<DataProtectionKeyContext>
{
    public DataProtectionKeyContext CreateDbContext(string[] args)
    {
        var optionBuilder = new DbContextOptionsBuilder<DataProtectionKeyContext>();
        optionBuilder.UseNpgsql();
        return new DataProtectionKeyContext(optionBuilder.Options);
    }
}