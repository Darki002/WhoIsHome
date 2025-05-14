using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WhoIsHome.Host.DataProtectionKeys;

public class DataProtectionKeyContext(DbContextOptions<DataProtectionKeyContext> options) : DbContext(options), 
    IDataProtectionKeyContext
{
    public virtual DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
}