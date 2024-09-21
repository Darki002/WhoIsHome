using Microsoft.EntityFrameworkCore;
using WhoIsHome.DataAccess.Models;

namespace WhoIsHome.DataAccess;

public class Context : DbContext
{
    public DbSet<User> Users { get; set; }
    
    public DbSet<Event> Events { get; set; }
    
    public DbSet<RepeatedEvent> RepeatedEvents { get; set; }
    
    public DbSet<DinnerTime> DinnerTimes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // TODO Add Connections String from Env Variable
        base.OnConfiguring(optionsBuilder);
    }
}