using WhoIsHome.Aggregates.Mappers;
using WhoIsHome.QueryHandler.DailyOverview;
using WhoIsHome.QueryHandler.WeeklyReports;
using WhoIsHome.Test.TestData;

namespace WhoIsHome.Test.Application.QueryHandler;

[TestFixture]
public class WeeklyReportTest : InMemoryDbTest
{
    private readonly DateTimeProviderFake dateTimeProviderFake = new();
    
    private WeeklyReportQueryHandler queryHandler;
    
    [SetUp]
    public void SetUp()
    {
        var userDayOverviewQueryHandler = new UserDayOverviewQueryHandler(DbFactory);
        var dailyOverviewQueryHandler = new DailyOverviewQueryHandler(DbFactory, userDayOverviewQueryHandler);
        queryHandler = new WeeklyReportQueryHandler(dailyOverviewQueryHandler, DbFactory, dateTimeProviderFake);
    }

    [Test]
    public async Task ReturnsReport_ForAllUsers()
    {
        // Arrange
        var user1 = UserTestData.CreateDefaultUser(email: "test1@whoishome.dev");
        var user2 = UserTestData.CreateDefaultUser(email: "test2@whoishome.dev");
        await Db.Users.AddAsync(user1.ToModel());
        await Db.Users.AddAsync(user2.ToModel());
        await Db.SaveChangesAsync();
        
        // Act
        var result = await queryHandler.HandleAsync(CancellationToken.None);
        
        // Asser
        result.Should().HaveCount(2);
        result[0].DailyOverviews.Should().HaveCount(7);
        result[1].DailyOverviews.Should().HaveCount(7);
    }

    [Test]
    public async Task ReturnsExpectedDailyOverview_WithOnlyOneTimeEvents()
    {
        // Arrange
        var expectedDinnerTime = new TimeOnly(20, 00, 00);

        var user1 = UserTestData.CreateDefaultUser(email: "test@whoishome.dev").ToModel();
        await Db.Users.AddAsync(user1);

        var oneTimeEvent = OneTimeEventTestData.CreateDefault(date: dateTimeProviderFake.CurrentDate,
                startTime: new TimeOnly(18, 00, 00),
                endTime: new TimeOnly(19, 00, 00), dinnerTime: expectedDinnerTime)
            .ToModel();
        await Db.OneTimeEvents.AddAsync(oneTimeEvent);

        await Db.SaveChangesAsync();
        
        // Act
        var result = await queryHandler.HandleAsync(CancellationToken.None);
        
        // Assert
        var (isAtHome, dinnerTime) = result.Single().DailyOverviews[dateTimeProviderFake.CurrentDate];
        dinnerTime.Should().Be(expectedDinnerTime);
        isAtHome.Should().BeTrue();
    }
    
    [Test]
    public async Task ReturnsExpectedDailyOverview_WithOnlyOneTimeEvents_WithInfinitelyEvent()
    {
        // Arrange
        var expectedDinnerTime = new TimeOnly(20, 00, 00);

        var user1 = UserTestData.CreateDefaultUser(email: "test@whoishome.dev").ToModel();
        await Db.Users.AddAsync(user1);

        var repeatedEvent = RepeatedEventTestData.CreateDefault(firstOccurrence: dateTimeProviderFake.CurrentDate,
                startTime: new TimeOnly(18, 00, 00),
                endTime: new TimeOnly(19, 00, 00), dinnerTime: expectedDinnerTime)
            .ToModel();
        await Db.RepeatedEvents.AddAsync(repeatedEvent);

        await Db.SaveChangesAsync();
        
        // Act
        var result = await queryHandler.HandleAsync(CancellationToken.None);
        
        // Assert
        var (isAtHome, dinnerTime) = result.Single().DailyOverviews[dateTimeProviderFake.CurrentDate];
        dinnerTime.Should().Be(expectedDinnerTime);
        isAtHome.Should().BeTrue();
    }
    
    [Test]
    public async Task ReturnsListOfAllDaysInCurrentWeek()
    {
        // Arrange
        var expectedFirstDate = new DateOnly(2024, 11, 25);
        var expectedLastDate = new DateOnly(2024, 12, 1);
        
        var user1 = UserTestData.CreateDefaultUser(email: "test@whoishome.dev").ToModel();
        await Db.Users.AddAsync(user1);
        await Db.SaveChangesAsync();
        
        // Act
        var result = await queryHandler.HandleAsync(CancellationToken.None);
        
        // Assert
        result.Should().HaveCount(1);
        result.Single().DailyOverviews.Should().HaveCount(7);
        result.Single().DailyOverviews.First().Key.Should().Be(expectedFirstDate);
        result.Single().DailyOverviews.Last().Key.Should().Be(expectedLastDate);
    }
}