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
        DbSetUp(DbMock);
    }

    // Optional Override for Set Up in test
    protected virtual void DbSetUp(Mock<WhoIsHomeContext> mock) { }
}