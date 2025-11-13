using Moq.EntityFrameworkCore;
using WhoIsHome.QueryHandler.DailyOverview;
using WhoIsHome.Test.TestData;

namespace WhoIsHome.Test.Application.QueryHandler;

[TestFixture]
public class DailyOverviewTest : DbMockTest
{
    private readonly DateTimeProviderFake dateTimeProviderFake = new();
    
    [SetUp]
    public void SetUp()
    {
        var handler = new UserDayOverviewQueryHandler(Db);
        queryHandler = new DailyOverviewQueryHandler(Db, handler);
    }

    private DailyOverviewQueryHandler queryHandler;

    [Test]
    public async Task ReturnsDailyOverview_ForEveryUser()
    {
        // Arrange
        var user1 = UserTestData.CreateDefaultUser(id: 1, email: "test1@whoishome.dev");
        var user2 = UserTestData.CreateDefaultUser(id: 2, email: "test2@whoishome.dev");
        DbMock.Setup(c => c.Users).ReturnsDbSet([user1, user2]);
        DbMock.Setup(c => c.EventInstances).ReturnsDbSet([]);

        // Act
        var result = await queryHandler.HandleAsync(dateTimeProviderFake.CurrentDate, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
    }

    [Test]
    public async Task ReturnsExpectedDailyOverview_FromGivenEvents()
    {
        // Arrange
        var expectedDinnerTime = new TimeOnly(20, 00, 00);

        var user = UserTestData.CreateDefaultUser(email: "test@whoishome.dev");
        DbMock.Setup(c => c.Users).ReturnsDbSet([user]);

        var eventInstance1 = EventInstanceTestData.CreateDefault(date: dateTimeProviderFake.CurrentDate);
        var eventInstance2 = EventInstanceTestData.CreateDefault(
            date: dateTimeProviderFake.CurrentDate,
            startTime: new TimeOnly(18, 00, 00),
            endTime: new TimeOnly(19, 00, 00), 
            dinnerTime: expectedDinnerTime);
        
        DbMock.Setup(c => c.EventInstances).ReturnsDbSet([eventInstance1, eventInstance2]);

        // Act
        var result = await queryHandler.HandleAsync(dateTimeProviderFake.CurrentDate, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.Single().IsAtHome.Should().BeTrue();
        result.Single().DinnerTime.Should().Be(expectedDinnerTime);
    }
    
    [Test]
    public async Task ReturnsExpectedDailyOverview_WithNotPresentNotToday()
    {
        // Arrange
        var user = UserTestData.CreateDefaultUser(email: "test@whoishome.dev");
        DbMock.Setup(c => c.Users).ReturnsDbSet([user]);
        
        var eventInstance1 = EventInstanceTestData.CreateDefault(date: dateTimeProviderFake.CurrentDate.AddDays(-2));
        var eventInstance2 = EventInstanceTestData.CreateDefault(date: dateTimeProviderFake.CurrentDate.AddDays(2));
        DbMock.Setup(c => c.EventInstances).ReturnsDbSet([eventInstance1, eventInstance2]);

        // Act
        var result = await queryHandler.HandleAsync(dateTimeProviderFake.CurrentDate, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.Single().IsAtHome.Should().BeTrue();
        result.Single().DinnerTime.Should().BeNull();
    }
    
    [Test]
    public async Task ReturnsNotAtHome_WhenEventTodayIsNotPresenceType()
    {
        // Arrange
        var user = UserTestData.CreateDefaultUser(email: "test@whoishome.dev");
        DbMock.Setup(c => c.Users).ReturnsDbSet([user]);

        var eventInstance = EventInstanceTestData.CreateWithNotPresent(date: dateTimeProviderFake.CurrentDate);
        DbMock.Setup(c => c.EventInstances).ReturnsDbSet([eventInstance]);

        // Act
        var result = await queryHandler.HandleAsync(dateTimeProviderFake.CurrentDate, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.Single().IsAtHome.Should().BeFalse();
        result.Single().DinnerTime.Should().BeNull();
    }
}