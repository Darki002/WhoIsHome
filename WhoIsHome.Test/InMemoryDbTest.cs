using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using WhoIsHome.DataAccess;

namespace WhoIsHome.Test;

public abstract class InMemoryDbTest
{
    protected WhoIsHomeContext Db { get; private set; }

    private DbContextOptions<WhoIsHomeContext> options = null!;

    [OneTimeSetUp]
    public void OneTimeSetUpDb()
    {
        options = new DbContextOptionsBuilder<WhoIsHomeContext>()
            .UseInMemoryDatabase("InMemoryDbTest")
            .ConfigureWarnings(c => c.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .EnableSensitiveDataLogging()
            .Options;
    }
    
    [SetUp]
    public async Task SetUpDb()
    {
        Db = new WhoIsHomeContext(options);

        await Db.Database.EnsureDeletedAsync();
        await Db.Database.EnsureCreatedAsync();
        
        await DbSetUpAsync();
    }

    // Optional Override for Set Up in test
    protected virtual Task DbSetUpAsync()
    {
        return Task.CompletedTask;
    }
}