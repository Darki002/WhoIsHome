using Moq.EntityFrameworkCore;
using WhoIsHome.QueryHandler.DailyOverview;
using WhoIsHome.QueryHandler.WeeklyReports;
using WhoIsHome.Test.TestData;

namespace WhoIsHome.Test.Application.QueryHandler;

[TestFixture]
public class WeeklyReportTest : DbMockTest
{
    private readonly DateTimeProviderFake dateTimeProviderFake = new DateTimeProviderFake();
    
    private WeeklyReportQueryHandler queryHandler;
    
    [SetUp]
    public void SetUp()
    {
        var userDayOverviewQueryHandler = new UserDayOverviewQueryHandler(Db);
        var dailyOverviewQueryHandler = new DailyOverviewQueryHandler(Db, userDayOverviewQueryHandler);
        queryHandler = new WeeklyReportQueryHandler(dailyOverviewQueryHandler, Db, dateTimeProviderFake);
    }

    [Test]
    public async Task ReturnsReport_ForAllUsers()
    {
        // Arrange
        var user1 = UserTestData.CreateDefaultUser(email: "test1@whoishome.dev");
        var user2 = UserTestData.CreateDefaultUser(email: "test2@whoishome.dev");
        DbMock.Setup(c => c.Users).ReturnsDbSet([user1, user2]);
        
        // Act
        var result = await queryHandler.HandleAsync(CancellationToken.None);
        
        // Asser
        result.Should().HaveCount(2);
        result[0].Report.Value.Should().HaveCount(7);
        result[1].Report.Value.Should().HaveCount(7);
    }

    [Test]
    public async Task ReturnsExpectedDailyOverview_WithOnlyOneTimeEvents()
    {
        // Arrange
        var expectedDinnerTime = new TimeOnly(20, 00, 00);

        var user = UserTestData.CreateDefaultUser(email: "test@whoishome.dev");
        DbMock.Setup(c => c.Users).ReturnsDbSet([user]);

        var evenInstance = EventInstanceTestData.CreateDefault(
            date: dateTimeProviderFake.CurrentDate,
            startTime: new TimeOnly(18, 00, 00),
            endTime: new TimeOnly(19, 00, 00), 
            dinnerTime: expectedDinnerTime);
        
        DbMock.Setup(c => c.EventInstances).ReturnsDbSet([evenInstance]);
        
        // Act
        var result = await queryHandler.HandleAsync(CancellationToken.None);
        
        // Assert
        var (isAtHome, dinnerTime) = result.Single().Report.Value[dateTimeProviderFake.CurrentDate];
        isAtHome.Should().BeTrue();
        dinnerTime.Should().Be(expectedDinnerTime);
    }
    
    [Test]
    public async Task ReturnsListOfAllDaysInCurrentWeek()
    {
        // Arrange
        var expectedFirstDate = new DateOnly(2024, 11, 25);
        var expectedLastDate = new DateOnly(2024, 12, 1);
        
        var user = UserTestData.CreateDefaultUser(email: "test@whoishome.dev");
        DbMock.Setup(c => c.Users).ReturnsDbSet([user]);
        
        // Act
        var result = await queryHandler.HandleAsync(CancellationToken.None);
        
        // Assert
        result.Should().HaveCount(1);
        result.Single().Report.Value.Should().HaveCount(7);
        result.Single().Report.Value.First().Key.Should().Be(expectedFirstDate);
        result.Single().Report.Value.Last().Key.Should().Be(expectedLastDate);
    }
}