using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WhoIsHome.AuthTokens;
using WhoIsHome.Entities;
using WhoIsHome.External.PushUp;

namespace WhoIsHome.External.Database;

public class WhoIsHomeContext(DbContextOptions<WhoIsHomeContext> options) : DbContext(options)
{
    public virtual DbSet<User> Users { get; set; }
    
    public virtual DbSet<EventTemplate> EventTemplates { get; set; }
    
    public virtual DbSet<EventInstance> EventInstances { get; set; }
    
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
    
    public virtual DbSet<PushUpSettings> PushUpSettings { get; set; }
    
    private static readonly ValueConverter<TimeOnly, TimeSpan> TimeOnlyConverter = new(
        only => only.ToTimeSpan(),
        span => TimeOnly.FromTimeSpan(span));

    private static readonly ValueConverter<DateOnly, DateTime> DateOnlyConverter = new(
        only => only.ToDateTime(TimeOnly.MinValue),
        dt => DateOnly.FromDateTime(dt));

    private static readonly ValueConverter<CultureInfo?, string?> CultureConverter = new(
        ci => ci != null ? ci.Name : null,
        s  => string.IsNullOrEmpty(s) ? null : new CultureInfo(s)
    );
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // EventTemplateModel
        modelBuilder.Entity<EventTemplate>()
            .Property(e => e.StartDate)
            .HasConversion(DateOnlyConverter);
        modelBuilder.Entity<EventTemplate>()
            .Property(e => e.EndDate)
            .HasConversion(
                d => d.HasValue ? d.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                d => d.HasValue ? DateOnly.FromDateTime(d.Value) : null);
    
        modelBuilder.Entity<EventTemplate>()
            .Property(e => e.StartTime)
            .HasConversion(TimeOnlyConverter);
    
        modelBuilder.Entity<EventTemplate>()
            .Property(e => e.EndTime)
            .HasConversion(
                t => t.HasValue ? t.Value.ToTimeSpan() : (TimeSpan?)null,
                t => t.HasValue ? TimeOnly.FromTimeSpan(t.Value) : null);
    
        modelBuilder.Entity<EventTemplate>()
            .Property(e => e.DinnerTime)
            .HasConversion(
                t => t.HasValue ? t.Value.ToTimeSpan() : (TimeSpan?)null,
                t => t.HasValue ? TimeOnly.FromTimeSpan(t.Value) : null);
        
        // EventModel
        modelBuilder.Entity<EventInstance>()
            .Property(e => e.Date)
            .HasConversion(DateOnlyConverter);
        
        modelBuilder.Entity<EventInstance>()
            .Property(e => e.OriginalDate)
            .HasConversion(DateOnlyConverter);
        
        modelBuilder.Entity<EventInstance>()
            .Property(e => e.StartTime)
            .HasConversion(TimeOnlyConverter);
    
        modelBuilder.Entity<EventInstance>()
            .Property(e => e.EndTime)
            .HasConversion(
                t => t.HasValue ? t.Value.ToTimeSpan() : (TimeSpan?)null,
                t => t.HasValue ? TimeOnly.FromTimeSpan(t.Value) : null);
    
        modelBuilder.Entity<EventInstance>()
            .Property(e => e.DinnerTime)
            .HasConversion(
                t => t.HasValue ? t.Value.ToTimeSpan() : (TimeSpan?)null,
                t => t.HasValue ? TimeOnly.FromTimeSpan(t.Value) : null);
        
        // PushUpSettings
        modelBuilder.Entity<PushUpSettings>()
            .Property(e => e.LanguageCode)
            .HasConversion(CultureConverter);
    }
}

// Used for EF Core Migrations
// ReSharper disable once UnusedType.Global
public class WhoIsHomeContextFactory : IDesignTimeDbContextFactory<WhoIsHomeContext>
{
    public WhoIsHomeContext CreateDbContext(string[] args)
    {
        var optionBuilder = new DbContextOptionsBuilder<WhoIsHomeContext>();
        optionBuilder.UseMySQL("Server=localhost;Database=local-whoishome;User=root;Password=1234");
        return new WhoIsHomeContext(optionBuilder.Options);
    }
}