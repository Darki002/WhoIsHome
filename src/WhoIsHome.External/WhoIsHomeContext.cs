using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WhoIsHome.External.Models;

namespace WhoIsHome.External;

public class WhoIsHomeContext(DbContextOptions<WhoIsHomeContext> options) : DbContext(options)
{
    public virtual DbSet<UserModel> Users { get; set; }
    
    public virtual DbSet<OneTimeEventModel> OneTimeEvents { get; set; }
    
    public virtual DbSet<RepeatedEventModel> RepeatedEvents { get; set; }
    
    public virtual DbSet<RefreshTokenModel> RefreshTokens { get; set; }
    
    public virtual DbSet<PushUpSettingsModel> PushUpSettings { get; set; }
    
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
        // OneTimeEventModel
        modelBuilder.Entity<OneTimeEventModel>()
            .Property(e => e.Date)
            .HasConversion(DateOnlyConverter);
    
        modelBuilder.Entity<OneTimeEventModel>()
            .Property(e => e.StartTime)
            .HasConversion(TimeOnlyConverter);
    
        modelBuilder.Entity<OneTimeEventModel>()
            .Property(e => e.EndTime)
            .HasConversion(
                t => t.HasValue ? t.Value.ToTimeSpan() : (TimeSpan?)null,
                t => t.HasValue ? TimeOnly.FromTimeSpan(t.Value) : null);
    
        modelBuilder.Entity<OneTimeEventModel>()
            .Property(e => e.DinnerTime)
            .HasConversion(
                t => t.HasValue ? t.Value.ToTimeSpan() : (TimeSpan?)null,
                t => t.HasValue ? TimeOnly.FromTimeSpan(t.Value) : null);
        
        // RepeatedEventModel
        modelBuilder.Entity<RepeatedEventModel>()
            .Property(e => e.FirstOccurrence)
            .HasConversion(DateOnlyConverter);
        
        modelBuilder.Entity<RepeatedEventModel>()
            .Property(e => e.LastOccurrence)
            .HasConversion(
                d => d.HasValue ? d.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                d => d.HasValue ? DateOnly.FromDateTime(d.Value) : null);
    
        modelBuilder.Entity<RepeatedEventModel>()
            .Property(e => e.StartTime)
            .HasConversion(TimeOnlyConverter);
    
        modelBuilder.Entity<RepeatedEventModel>()
            .Property(e => e.EndTime)
            .HasConversion(
                t => t.HasValue ? t.Value.ToTimeSpan() : (TimeSpan?)null,
                t => t.HasValue ? TimeOnly.FromTimeSpan(t.Value) : null);
    
        modelBuilder.Entity<RepeatedEventModel>()
            .Property(e => e.DinnerTime)
            .HasConversion(
                t => t.HasValue ? t.Value.ToTimeSpan() : (TimeSpan?)null,
                t => t.HasValue ? TimeOnly.FromTimeSpan(t.Value) : null);
        
        // PushUpSettings
        modelBuilder.Entity<PushUpSettingsModel>()
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