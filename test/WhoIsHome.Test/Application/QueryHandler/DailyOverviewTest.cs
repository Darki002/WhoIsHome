using WhoIsHome.Aggregates.Mappers;
using WhoIsHome.QueryHandler.DailyOverview;
using WhoIsHome.Shared.Helper;
using WhoIsHome.Test.TestData;

namespace WhoIsHome.Test.Application.QueryHandler;

[TestFixture]
public class DailyOverviewTest : InMemoryDbTest
{
    [SetUp]
    public void SetUp()
    {
        queryHandler = new DailyOverviewQueryHandler(Db);
    }

    private DailyOverviewQueryHandler queryHandler;

    [Test]
    public async Task ReturnsDailyOverview_ForEveryPerson()
    {
        // Arrange
        var user1 = UserTestData.CreateDefaultUser(email: "test1@whoishome.dev");
        var user2 = UserTestData.CreateDefaultUser(email: "test2@whoishome.dev");
        await Db.Users.AddAsync(user1.ToModel());
        await Db.Users.AddAsync(user2.ToModel());
        await Db.SaveChangesAsync();

        // Act
        var result = await queryHandler.HandleAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
    }

    [Test]
    public async Task ReturnsExpectedDailyOverview_WithOnlyOneTimeEvents()
    {
        // Arrange
        var expectedDinnerTime = new TimeOnly(20, 00, 00);

        var user1 = UserTestData.CreateDefaultUser(email: "test@whoishome.dev").ToModel();
        await Db.Users.AddAsync(user1);

        var oneTimeEvent1 = OneTimeEventTestData.CreateDefault(date: DateOnlyHelper.Today).ToModel(user1);
        await Db.OneTimeEvents.AddAsync(oneTimeEvent1);

        var oneTimeEvent2 = OneTimeEventTestData.CreateDefault(date: DateOnlyHelper.Today,
                startTime: new TimeOnly(18, 00, 00),
                endTime: new TimeOnly(19, 00, 00), dinnerTime: expectedDinnerTime)
            .ToModel(user1);
        await Db.OneTimeEvents.AddAsync(oneTimeEvent2);

        await Db.SaveChangesAsync();

        // Act
        var result = await queryHandler.HandleAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.Single().IsAtHome.Should().BeTrue();
        result.Single().DinnerTime.Should().Be(expectedDinnerTime);
    }

    [Test]
    public async Task ReturnsExpectedDailyOverview_WithOnlyRepeatedEvents()
    {
        // Arrange
        var expectedDinnerTime = new TimeOnly(20, 00, 00);

        var user1 = UserTestData.CreateDefaultUser(email: "test@whoishome.dev").ToModel();
        await Db.Users.AddAsync(user1);

        var repeatedEvent1 = RepeatedEventTestData
            .CreateDefault(firstOccurrence: DateOnlyHelper.Today, lastOccurrence: DateOnlyHelper.Today.AddDays(7))
            .ToModel(user1);
        await Db.RepeatedEvents.AddAsync(repeatedEvent1);

        var repeatedEvent2 = RepeatedEventTestData.CreateDefault(firstOccurrence: DateOnlyHelper.Today,
                lastOccurrence: DateOnlyHelper.Today.AddDays(7), startTime: new TimeOnly(18, 00, 00),
                endTime: new TimeOnly(19, 00, 00), dinnerTime: expectedDinnerTime)
            .ToModel(user1);
        await Db.RepeatedEvents.AddAsync(repeatedEvent2);

        await Db.SaveChangesAsync();

        // Act
        var result = await queryHandler.HandleAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.Single().IsAtHome.Should().BeTrue();
        result.Single().DinnerTime.Should().Be(expectedDinnerTime);
    }
    
    [Test]
    public async Task ReturnsExpectedDailyOverview_WithMixedEvents()
    {
        // Arrange
        var expectedDinnerTime = new TimeOnly(20, 00, 00);

        var user1 = UserTestData.CreateDefaultUser(email: "test@whoishome.dev").ToModel();
        await Db.Users.AddAsync(user1);

        var oneTimeEvent = OneTimeEventTestData
            .CreateDefault(date: DateOnlyHelper.Today)
            .ToModel(user1);
        await Db.OneTimeEvents.AddAsync(oneTimeEvent);

        var repeatedEvent = RepeatedEventTestData.CreateDefault(firstOccurrence: DateOnlyHelper.Today,
                lastOccurrence: DateOnlyHelper.Today.AddDays(7), startTime: new TimeOnly(18, 00, 00),
                endTime: new TimeOnly(19, 00, 00), dinnerTime: expectedDinnerTime)
            .ToModel(user1);
        await Db.RepeatedEvents.AddAsync(repeatedEvent);

        await Db.SaveChangesAsync();

        // Act
        var result = await queryHandler.HandleAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.Single().IsAtHome.Should().BeTrue();
        result.Single().DinnerTime.Should().Be(expectedDinnerTime);
    }
}