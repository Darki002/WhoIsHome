using Moq;
using WhoIsHome.External.Database;

namespace WhoIsHome.Test;

public abstract class DbMockTest
{
    protected WhoIsHomeContext Db => DbMock.Object;
    
    protected Mock<WhoIsHomeContext> DbMock { get; private set; }
    
    [SetUp]
    public void SetUpDb()
    {
        DbMock = new Mock<WhoIsHomeContext>();
        DbSetUpAsync(DbMock);
    }

    // Optional Override for Set Up in test
    protected virtual void DbSetUpAsync(Mock<WhoIsHomeContext> mock) { }
}