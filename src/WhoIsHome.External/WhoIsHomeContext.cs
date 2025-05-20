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
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var timeOnlyConverter = new ValueConverter<TimeOnly, TimeSpan>(
            only => only.ToTimeSpan(),
            span => TimeOnly.FromTimeSpan(span));
        
        var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
            only => only.ToDateTime(TimeOnly.MinValue),
            dt => DateOnly.FromDateTime(dt));
        
        var cultureConverter = new ValueConverter<CultureInfo,string>(
            ci => ci.Name,
            s  => new CultureInfo(s)
        );
        
        // OneTimeEventModel
        modelBuilder.Entity<OneTimeEventModel>()
            .Property(e => e.Date)
            .HasConversion(dateOnlyConverter);
    
        modelBuilder.Entity<OneTimeEventModel>()
            .Property(e => e.StartTime)
            .HasConversion(timeOnlyConverter);
    
        modelBuilder.Entity<OneTimeEventModel>()
            .Property(e => e.EndTime)
            .HasConversion(timeOnlyConverter);
    
        modelBuilder.Entity<OneTimeEventModel>()
            .Property(e => e.DinnerTime)
            .HasConversion(
                t => t.HasValue ? t.Value.ToTimeSpan() : (TimeSpan?)null,
                t => t.HasValue ? TimeOnly.FromTimeSpan(t.Value) : null);
        
        // RepeatedEventModel
        modelBuilder.Entity<RepeatedEventModel>()
            .Property(e => e.FirstOccurrence)
            .HasConversion(timeOnlyConverter);
        
        modelBuilder.Entity<RepeatedEventModel>()
            .Property(e => e.LastOccurrence)
            .HasConversion(timeOnlyConverter);
    
        modelBuilder.Entity<RepeatedEventModel>()
            .Property(e => e.StartTime)
            .HasConversion(timeOnlyConverter);
    
        modelBuilder.Entity<RepeatedEventModel>()
            .Property(e => e.EndTime)
            .HasConversion(timeOnlyConverter);
    
        modelBuilder.Entity<RepeatedEventModel>()
            .Property(e => e.DinnerTime)
            .HasConversion(
                t => t.HasValue ? t.Value.ToTimeSpan() : (TimeSpan?)null,
                t => t.HasValue ? TimeOnly.FromTimeSpan(t.Value) : null);
        
        // PushUpSettings
        modelBuilder.Entity<PushUpSettingsModel>()
            .Property(e => e.LanguageCode)
            .HasConversion(cultureConverter);
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