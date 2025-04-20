using WhoIsHome.Aggregates.Mappers;
using WhoIsHome.QueryHandler.DailyOverview;
using WhoIsHome.QueryHandler.WeeklyReports;
using WhoIsHome.Shared.Helper;
using WhoIsHome.Test.TestData;

namespace WhoIsHome.Test.Application.QueryHandler;

[TestFixture]
public class WeeklyReportTest : InMemoryDbTest
{
    private readonly DateTimeProviderFake dateTimeProviderFake = new();
    
    [SetUp]
    public void SetUp()
    {
        var userDayOverviewQueryHandler = new UserDayOverviewQueryHandler(DbFactory);
        var dailyOverviewQueryHandler = new DailyOverviewQueryHandler(DbFactory, userDayOverviewQueryHandler);
        queryHandler = new WeeklyReportQueryHandler(dailyOverviewQueryHandler, DbFactory, dateTimeProviderFake);
    }
    
    private WeeklyReportQueryHandler queryHandler;

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
        var expectedDate = DateOnly.FromDateTime(dateTimeProviderFake.Now.StartOfWeek());

        var user1 = UserTestData.CreateDefaultUser(email: "test@whoishome.dev").ToModel();
        await Db.Users.AddAsync(user1);

        var oneTimeEvent1 = OneTimeEventTestData.CreateDefault(date: dateTimeProviderFake.CurrentDate).ToModel();
        await Db.OneTimeEvents.AddAsync(oneTimeEvent1);

        var oneTimeEvent2 = OneTimeEventTestData.CreateDefault(date: dateTimeProviderFake.CurrentDate,
                startTime: new TimeOnly(18, 00, 00),
                endTime: new TimeOnly(19, 00, 00), dinnerTime: expectedDinnerTime)
            .ToModel();
        await Db.OneTimeEvents.AddAsync(oneTimeEvent2);

        await Db.SaveChangesAsync();
        
        // Act
        var result = await queryHandler.HandleAsync(CancellationToken.None);
        
        // Assert
        result.Should().HaveCount(1);
        result.Single().DailyOverviews.Should().HaveCount(7);
        result.Single().DailyOverviews.First().Key.Should().Be(expectedDate);
        result.Single().DailyOverviews.First().Value.DinnerTime.Should().Be(expectedDinnerTime);
        result.Single().DailyOverviews.First().Value.IsAtHome.Should().BeTrue();
    }
}