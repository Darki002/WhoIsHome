using Microsoft.EntityFrameworkCore;
using WhoIsHome.DataAccess.Models;

namespace WhoIsHome.DataAccess;

public class WhoIsHomeContext : DbContext
{
    public DbSet<UserModel> Users { get; set; }
    
    public DbSet<OneTimeEventModel> Events { get; set; }
    
    public DbSet<RepeatedEventModel> RepeatedEvents { get; set; }
    
    public DbSet<DinnerTimeModel> DinnerTimes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // TODO Add Connections String from Env Variable
        base.OnConfiguring(optionsBuilder);
    }
}