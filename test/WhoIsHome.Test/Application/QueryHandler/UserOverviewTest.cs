using Moq.EntityFrameworkCore;
using WhoIsHome.QueryHandler.UserOverview;
using WhoIsHome.Test.TestData;

namespace WhoIsHome.Test.Application.QueryHandler;

[TestFixture]
public class UserOverviewTest : DbMockTest
{
    private readonly DateTimeProviderFake dateTimeProviderFake = new DateTimeProviderFake();
    
    [SetUp]
    public void SetUp()
    {
        queryHandler = new UserOverviewQueryHandler(Db, dateTimeProviderFake);
    }

    private UserOverviewQueryHandler queryHandler;

    [Test]
    public async Task ReturnsOverview_FromExpectedUser()
    {
        // Arrange
        var user1 = UserTestData.CreateDefaultUser();
        var user2 = UserTestData.CreateDefaultUser(id: 2);
        DbMock.Setup(c => c.Users).ReturnsDbSet([user1, user2]);

        // Act
        var result = await queryHandler.HandleAsync(1, CancellationToken.None);

        // Assert
        result.User.Id.Should().Be(1);
    }

    [Test]
    public async Task ReturnsOverview_WithExpectedEvents_ForToday()
    {
        // Arrange
        var user = UserTestData.CreateDefaultUser();
        DbMock.Setup(c => c.Users).ReturnsDbSet([user]);
        
        var eventInstance1 = EventInstanceTestData.CreateDefault(title: "1", date: dateTimeProviderFake.CurrentDate);
        var eventInstance2 = EventInstanceTestData.CreateDefault(title: "2", date: dateTimeProviderFake.CurrentDate.AddDays(-1));
        var eventInstance3 = EventInstanceTestData.CreateDefault(title: "3", date: dateTimeProviderFake.CurrentDate.AddDays(1));
        var eventInstance4 = EventInstanceTestData.CreateDefault(title: "4", date: dateTimeProviderFake.CurrentDate);
        DbMock.Setup(c => c.EventInstances).ReturnsDbSet([eventInstance1, eventInstance2, eventInstance3, eventInstance4]);
        
        // Act
        var result = await queryHandler.HandleAsync(1, CancellationToken.None);
        
        // Assert
        result.Today.Should().HaveCount(2);
        result.Today.Should().ContainSingle(e => e.Title == "1");
        result.Today.Should().ContainSingle(e => e.Title == "4");
    }
    
    [Test]
    public async Task ReturnsOverview_WithExpectedEvents_ForThisWeek()
    {
        // Arrange
        var user = UserTestData.CreateDefaultUser();
        DbMock.Setup(c => c.Users).ReturnsDbSet([user]);
        
        var eventInstance1 = EventInstanceTestData.CreateDefault(title: "1", date: dateTimeProviderFake.CurrentDate);
        var eventInstance2 = EventInstanceTestData.CreateDefault(title: "2", date: dateTimeProviderFake.CurrentDate.AddDays(1));
        var eventInstance3 = EventInstanceTestData.CreateDefault(title: "3", date: dateTimeProviderFake.CurrentDate.AddDays(-1));
        var eventInstance4 = EventInstanceTestData.CreateDefault(title: "4", date: dateTimeProviderFake.CurrentDate.AddDays(1));
        DbMock.Setup(c => c.EventInstances).ReturnsDbSet([eventInstance1, eventInstance2, eventInstance3, eventInstance4]);
        
        // Act
        var result = await queryHandler.HandleAsync(1, CancellationToken.None);
        
        // Assert
        result.ThisWeek.Should().HaveCount(2);
        result.ThisWeek.Should().ContainSingle(e => e.Title == "2");
        result.ThisWeek.Should().ContainSingle(e => e.Title == "4");
    }
    
    [Test]
    public async Task ReturnsOverview_WithExpectedEvents_ForFutureEvents()
    {
        // Arrange
        var user = UserTestData.CreateDefaultUser();
        DbMock.Setup(c => c.Users).ReturnsDbSet([user]);
        
        var eventInstance1 = EventInstanceTestData.CreateDefault(title: "1", date: dateTimeProviderFake.CurrentDate);
        var eventInstance2 = EventInstanceTestData.CreateDefault(title: "2", date: dateTimeProviderFake.CurrentDate.AddDays(-1));
        var eventInstance3 = EventInstanceTestData.CreateDefault(title: "3", date: dateTimeProviderFake.CurrentDate.AddDays(8));
        DbMock.Setup(c => c.EventInstances).ReturnsDbSet([eventInstance1, eventInstance2, eventInstance3]);
        
        // Act
        var result = await queryHandler.HandleAsync(1, CancellationToken.None);
        
        // Assert
        result.FutureEvents.Should().HaveCount(1);
        result.FutureEvents.Should().ContainSingle(e => e.Title == "3");
    }
}