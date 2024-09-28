using Microsoft.EntityFrameworkCore;
using WhoIsHome.DataAccess.Models;

namespace WhoIsHome.DataAccess;

public class WhoIsHomeContext(DbContextOptions<WhoIsHomeContext> options) : DbContext(options)
{
    public DbSet<UserModel> Users { get; set; }
    
    public DbSet<OneTimeEventModel> OneTimeEvents { get; set; }
    
    public DbSet<RepeatedEventModel> RepeatedEvents { get; set; }
    
    public DbSet<DinnerTimeModel> DinnerTimes { get; set; }
} 