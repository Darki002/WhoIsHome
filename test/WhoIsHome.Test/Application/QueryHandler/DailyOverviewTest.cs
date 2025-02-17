using WhoIsHome.Aggregates.Mappers;
using WhoIsHome.QueryHandler.DailyOverview;
using WhoIsHome.Shared.Helper;
using WhoIsHome.Shared.Types;
using WhoIsHome.Test.TestData;

namespace WhoIsHome.Test.Application.QueryHandler;

[TestFixture]
public class DailyOverviewTest : InMemoryDbTest
{
    private readonly DateTimeProviderFake dateTimeProviderFake = new DateTimeProviderFake();
    
    [SetUp]
    public void SetUp()
    {
        queryHandler = new DailyOverviewQueryHandler(DbFactory, dateTimeProviderFake);
    }

    private DailyOverviewQueryHandler queryHandler;

    [Test]
    public async Task ReturnsDailyOverview_ForEveryUser()
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
            .CreateDefault(firstOccurrence: dateTimeProviderFake.CurrentDate, lastOccurrence: dateTimeProviderFake.CurrentDate.AddDays(7))
            .ToModel();
        await Db.RepeatedEvents.AddAsync(repeatedEvent1);

        var repeatedEvent2 = RepeatedEventTestData.CreateDefault(firstOccurrence: dateTimeProviderFake.CurrentDate,
                lastOccurrence: dateTimeProviderFake.CurrentDate.AddDays(7), startTime: new TimeOnly(18, 00, 00),
                endTime: new TimeOnly(19, 00, 00), dinnerTime: expectedDinnerTime)
            .ToModel();
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
            .CreateDefault(date: dateTimeProviderFake.CurrentDate)
            .ToModel();
        await Db.OneTimeEvents.AddAsync(oneTimeEvent);

        var repeatedEvent = RepeatedEventTestData.CreateDefault(firstOccurrence: dateTimeProviderFake.CurrentDate,
                lastOccurrence: dateTimeProviderFake.CurrentDate.AddDays(7), startTime: new TimeOnly(18, 00, 00),
                endTime: new TimeOnly(19, 00, 00), dinnerTime: expectedDinnerTime)
            .ToModel();
        await Db.RepeatedEvents.AddAsync(repeatedEvent);

        await Db.SaveChangesAsync();

        // Act
        var result = await queryHandler.HandleAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.Single().IsAtHome.Should().BeTrue();
        result.Single().DinnerTime.Should().Be(expectedDinnerTime);
    }
    
    [Test]
    public async Task ReturnsNotAtHome_WhenTheOnlyEventTodayIsHasNotPresenceType()
    {
        // Arrange
        var user1 = UserTestData.CreateDefaultUser(email: "test@whoishome.dev").ToModel();
        await Db.Users.AddAsync(user1);

        var oneTimeEvent = OneTimeEventTestData
            .CreateWithNotPresent(date: dateTimeProviderFake.CurrentDate)
            .ToModel();
        await Db.OneTimeEvents.AddAsync(oneTimeEvent);

        await Db.SaveChangesAsync();

        // Act
        var result = await queryHandler.HandleAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.Single().IsAtHome.Should().BeFalse();
        result.Single().DinnerTime.Should().BeNull();
    }
    
    [Test]
    public async Task ReturnsNotAtHome_WhenAnyEventTodayIsHasNotPresenceType()
    {
        // Arrange
        var user1 = UserTestData.CreateDefaultUser(email: "test@whoishome.dev").ToModel();
        await Db.Users.AddAsync(user1);

        var oneTimeEvent = OneTimeEventTestData
            .CreateWithNotPresent(date: dateTimeProviderFake.CurrentDate)
            .ToModel();
        await Db.OneTimeEvents.AddAsync(oneTimeEvent);
        
        var oneTimeEvent2 = OneTimeEventTestData
            .CreateDefault(date: dateTimeProviderFake.CurrentDate)
            .ToModel();
        await Db.OneTimeEvents.AddAsync(oneTimeEvent2);

        await Db.SaveChangesAsync();

        // Act
        var result = await queryHandler.HandleAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.Single().IsAtHome.Should().BeFalse();
        result.Single().DinnerTime.Should().BeNull();
    }
}