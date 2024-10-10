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
            .Options;
    }
    
    [SetUp]
    public async Task SetUpDb()
    {
        Db = new WhoIsHomeContext(options);
        await DbSetUpAsync(Db);
    }

    // Optional Override for Set Up in test
    protected virtual Task DbSetUpAsync(WhoIsHomeContext context)
    {
        return Task.CompletedTask;
    }
}