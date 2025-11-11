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
        var user1 = UserTestData.CreateDefaultUser(email: "test1@whoishome.dev");
        var user2 = UserTestData.CreateDefaultUser(email: "test2@whoishome.dev");
        DbMock.Setup(c => c.Users).ReturnsDbSet([user1, user2]);

        // Act
        var result = await queryHandler.HandleAsync(dateTimeProviderFake.CurrentDate, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
    }

    [Test]
    public async Task ReturnsExpectedDailyOverview_WithOnlyOneTimeEvents()
    {
        // Arrange
        var expectedDinnerTime = new TimeOnly(20, 00, 00);

        var user1 = UserTestData.CreateDefaultUser(email: "test@whoishome.dev");
        DbMock.Setup(c => c.Users).ReturnsDbSet([user1]);

        var eventInstance1 = EventInstanceTestData.CreateDefault(date: dateTimeProviderFake.CurrentDate);
        var eventInstance2 = EventInstanceTestData.CreateDefault(date: dateTimeProviderFake.CurrentDate,
                startTime: new TimeOnly(18, 00, 00),
                endTime: new TimeOnly(19, 00, 00), dinnerTime: expectedDinnerTime);
        
        DbMock.Setup(c => c.EventInstances).ReturnsDbSet([eventInstance1, eventInstance2]);

        // Act
        var result = await queryHandler.HandleAsync(dateTimeProviderFake.CurrentDate, CancellationToken.None);

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

        var user1 = UserTestData.CreateDefaultUser(email: "test@whoishome.dev");
        DbMock.Setup(c => c.Users).ReturnsDbSet([user1]);

        var repeatedEvent1 = RepeatedEventTestData
            .CreateDefaultWithDefaultDateTimes(firstOccurrence: dateTimeProviderFake.CurrentDate, 
                lastOccurrence: dateTimeProviderFake.CurrentDate.AddDays(7), 
                userId: 1);
        var repeatedEvent2 = RepeatedEventTestData.CreateDefaultWithDefaultDateTimes(firstOccurrence: dateTimeProviderFake.CurrentDate,
                lastOccurrence: dateTimeProviderFake.CurrentDate.AddDays(7), startTime: new TimeOnly(18, 00, 00),
                endTime: new TimeOnly(19, 00, 00), 
                dinnerTime: expectedDinnerTime,
                userId: 1);
        
        DbMock.Setup(c => c.EventInstances).ReturnsDbSet([repeatedEvent1, repeatedEvent2]);

        // Act
        var result = await queryHandler.HandleAsync(dateTimeProviderFake.CurrentDate, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.Single().User.Id.Should().Be(1);
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

        var oneTimeEvent = EventInstanceTestData
            .CreateDefault(date: dateTimeProviderFake.CurrentDate)
            .ToModel();
        await Db.OneTimeEvents.AddAsync(oneTimeEvent);

        var repeatedEvent = RepeatedEventTestData.CreateDefaultWithDefaultDateTimes(firstOccurrence: dateTimeProviderFake.CurrentDate,
                lastOccurrence: dateTimeProviderFake.CurrentDate.AddDays(7), startTime: new TimeOnly(18, 00, 00),
                endTime: new TimeOnly(19, 00, 00), dinnerTime: expectedDinnerTime)
            .ToModel();
        await Db.RepeatedEvents.AddAsync(repeatedEvent);

        await Db.SaveChangesAsync();

        // Act
        var result = await queryHandler.HandleAsync(dateTimeProviderFake.CurrentDate, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.Single().IsAtHome.Should().BeTrue();
        result.Single().DinnerTime.Should().Be(expectedDinnerTime);
    }
    
    [Test]
    public async Task ReturnsExpectedDailyOverview_WithNotPresentRepeatingNotToday()
    {
        // Arrange
        var user = UserTestData.CreateDefaultUser(email: "test@whoishome.dev").ToModel();
        await Db.Users.AddAsync(user);

        var repeatedEvent = RepeatedEventTestData.NotPresentWithDefaultDateTimes(
                firstOccurrence: dateTimeProviderFake.CurrentDate.AddDays(-2),
                lastOccurrence: dateTimeProviderFake.CurrentDate.AddDays(5), 
                startTime: new TimeOnly(18, 00, 00),
                endTime: new TimeOnly(19, 00, 00))
            .ToModel();
        await Db.RepeatedEvents.AddAsync(repeatedEvent);

        await Db.SaveChangesAsync();

        // Act
        var result = await queryHandler.HandleAsync(dateTimeProviderFake.CurrentDate, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.Single().IsAtHome.Should().BeTrue();
        result.Single().DinnerTime.Should().BeNull();
    }
    
    [Test]
    public async Task ReturnsNotAtHome_WhenTheOnlyEventTodayIsHasNotPresenceType()
    {
        // Arrange
        var user1 = UserTestData.CreateDefaultUser(email: "test@whoishome.dev").ToModel();
        await Db.Users.AddAsync(user1);

        var oneTimeEvent = EventInstanceTestData
            .CreateWithNotPresent(date: dateTimeProviderFake.CurrentDate)
            .ToModel();
        await Db.OneTimeEvents.AddAsync(oneTimeEvent);

        await Db.SaveChangesAsync();

        // Act
        var result = await queryHandler.HandleAsync(dateTimeProviderFake.CurrentDate, CancellationToken.None);

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

        var oneTimeEvent = EventInstanceTestData
            .CreateWithNotPresent(date: dateTimeProviderFake.CurrentDate)
            .ToModel();
        await Db.OneTimeEvents.AddAsync(oneTimeEvent);
        
        var oneTimeEvent2 = EventInstanceTestData
            .CreateDefault(date: dateTimeProviderFake.CurrentDate)
            .ToModel();
        await Db.OneTimeEvents.AddAsync(oneTimeEvent2);

        await Db.SaveChangesAsync();

        // Act
        var result = await queryHandler.HandleAsync(dateTimeProviderFake.CurrentDate, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.Single().IsAtHome.Should().BeFalse();
        result.Single().DinnerTime.Should().BeNull();
    }
    
    [Test]
    public async Task ReturnsExpectedDailyOverview_WithNotPresentRepeatingInfinitely()
    {
        // Arrange
        var user = UserTestData.CreateDefaultUser(email: "test@whoishome.dev").ToModel();
        await Db.Users.AddAsync(user);

        var repeatedEvent = RepeatedEventTestData.NotPresent(
                firstOccurrence: dateTimeProviderFake.CurrentDate.AddDays(-7),
                startTime: new TimeOnly(18, 00, 00),
                endTime: new TimeOnly(19, 00, 00))
            .ToModel();
        await Db.RepeatedEvents.AddAsync(repeatedEvent);

        await Db.SaveChangesAsync();

        // Act
        var result = await queryHandler.HandleAsync(dateTimeProviderFake.CurrentDate, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.Single().IsAtHome.Should().BeFalse();
        result.Single().DinnerTime.Should().BeNull();
    }
}