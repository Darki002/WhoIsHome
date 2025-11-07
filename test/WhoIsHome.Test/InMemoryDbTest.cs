using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using WhoIsHome.External;
using WhoIsHome.External.Database;

namespace WhoIsHome.Test;

public abstract class InMemoryDbTest
{
    protected WhoIsHomeContext Db { get; private set; }

    protected IDbContextFactory<WhoIsHomeContext> DbFactory { get; private set; }

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

        var factoryMock = new Mock<IDbContextFactory<WhoIsHomeContext>>();
        factoryMock
            .Setup(d => d.CreateDbContextAsync(CancellationToken.None))
            .ReturnsAsync(() => Db);
        DbFactory = factoryMock.Object;
    }

    // Optional Override for Set Up in test
    protected virtual Task DbSetUpAsync()
    {
        return Task.CompletedTask;
    }
}