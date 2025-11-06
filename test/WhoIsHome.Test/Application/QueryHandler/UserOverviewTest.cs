using WhoIsHome.QueryHandler.UserOverview;
using WhoIsHome.Test.Shared.Helper;
using WhoIsHome.Test.TestData;

namespace WhoIsHome.Test.Application.QueryHandler;

[TestFixture]
public class UserOverviewTest : InMemoryDbTest
{
    private readonly DateTimeProviderFake dateTimeProviderFake = new DateTimeProviderFake();
    
    [SetUp]
    public void SetUp()
    {
        queryHandler = new UserOverviewQueryHandler(DbFactory, dateTimeProviderFake);
    }

    private UserOverviewQueryHandler queryHandler;

    [Test]
    public async Task ReturnsOverview_FromExpectedUser()
    {
        // Arrange
        var user = UserTestData.CreateDefaultUser();
        await Db.Users.AddAsync(user.ToModel());
        await Db.SaveChangesAsync();

        // Act
        var result = await queryHandler.HandleAsync(1, CancellationToken.None);

        // Assert
        result.User.Id.Should().Be(1);
    }

    [Test]
    public async Task ReturnsOverview_WithExpectedEvents_ForToday()
    {
        // Arrange
        var user = UserTestData.CreateDefaultUser().ToModel();
        await Db.Users.AddAsync(user);
        
        var oneTimeEvent1 = OneTimeEventTestData.CreateDefault(title: "1", date: dateTimeProviderFake.CurrentDate).ToModel();
        var oneTimeEvent2 = OneTimeEventTestData.CreateDefault(title: "2", date: dateTimeProviderFake.CurrentDate.AddDays(-1)).ToModel();
        var oneTimeEvent3 = OneTimeEventTestData.CreateDefault(title: "3", date: dateTimeProviderFake.CurrentDate.AddDays(1)).ToModel();
        
        var repeatedEvent1 = RepeatedEventTestData
            .CreateDefaultWithDefaultDateTimes(title: "4", firstOccurrence: dateTimeProviderFake.CurrentDate, lastOccurrence: dateTimeProviderFake.CurrentDate.AddDays(7))
            .ToModel();
        var repeatedEvent2 = RepeatedEventTestData
            .CreateDefaultWithDefaultDateTimes(title: "5", firstOccurrence: dateTimeProviderFake.CurrentDate.AddDays(7), lastOccurrence: dateTimeProviderFake.CurrentDate.AddDays(14))
            .ToModel();
        var repeatedEvent3 = RepeatedEventTestData
            .CreateDefaultWithDefaultDateTimes(title: "6", firstOccurrence: dateTimeProviderFake.CurrentDate.AddDays(-14), lastOccurrence: dateTimeProviderFake.CurrentDate.AddDays(-7))
            .ToModel();

        await Db.AddRangeAsync(oneTimeEvent1, oneTimeEvent2, oneTimeEvent3, repeatedEvent1, repeatedEvent2, repeatedEvent3);
        await Db.SaveChangesAsync();
        
        // Act
        var result = await queryHandler.HandleAsync(1, CancellationToken.None);
        
        // Assert
        result.Today.Should().HaveCount(2);
        foreach (var userOverviewEvent in result.Today)
        {
            Console.WriteLine(userOverviewEvent.Title);
        }
        result.Today.Should().ContainSingle(e => e.Title == "1");
        result.Today.Should().ContainSingle(e => e.Title == "4");
    }
    
    [Test]
    public async Task ReturnsOverview_WithExpectedEvents_ForToday_WithInfinitelyEvent()
    {
        // Arrange
        var user = UserTestData.CreateDefaultUser().ToModel();
        await Db.Users.AddAsync(user);
        
        
        var repeatedEvent = RepeatedEventTestData
            .CreateDefault(title: "6", firstOccurrence: dateTimeProviderFake.CurrentDate.AddDays(-14))
            .ToModel();

        await Db.AddRangeAsync(repeatedEvent);
        await Db.SaveChangesAsync();
        
        // Act
        var result = await queryHandler.HandleAsync(1, CancellationToken.None);
        
        // Assert
        result.Today.Should().HaveCount(1);
        result.Today.Should().ContainSingle(e => e.Title == "6");
    }
    
    [Test]
    public async Task ReturnsOverview_WithExpectedEvents_ForThisWeek()
    {
        // Arrange
        var user = UserTestData.CreateDefaultUser().ToModel();
        await Db.Users.AddAsync(user);
        
        var oneTimeEvent1 = OneTimeEventTestData.CreateDefault(title: "1", date: dateTimeProviderFake.CurrentDate).ToModel();
        var oneTimeEvent2 = OneTimeEventTestData.CreateDefault(title: "2", date: dateTimeProviderFake.CurrentDate.AddDays(1)).ToModel();
        var oneTimeEvent3 = OneTimeEventTestData.CreateDefault(title: "3", date: dateTimeProviderFake.CurrentDate.AddDays(-1)).ToModel();
        
        var repeatedEvent1 = RepeatedEventTestData
            .CreateDefaultWithDefaultDateTimes(title: "4", firstOccurrence: dateTimeProviderFake.CurrentDate, lastOccurrence: dateTimeProviderFake.CurrentDate.AddDays(7))
            .ToModel();
        var repeatedEvent2 = RepeatedEventTestData
            .CreateDefaultWithDefaultDateTimes(title: "5", firstOccurrence: dateTimeProviderFake.CurrentDate.AddDays(2), lastOccurrence: dateTimeProviderFake.CurrentDate.AddDays(14))
            .ToModel();
        var repeatedEvent3 = RepeatedEventTestData
            .CreateDefaultWithDefaultDateTimes(title: "6", firstOccurrence: dateTimeProviderFake.CurrentDate.AddDays(-14), lastOccurrence: dateTimeProviderFake.CurrentDate.AddDays(-7))
            .ToModel();

        await Db.AddRangeAsync(oneTimeEvent1, oneTimeEvent2, oneTimeEvent3, repeatedEvent1, repeatedEvent2, repeatedEvent3);
        await Db.SaveChangesAsync();
        
        // Act
        var result = await queryHandler.HandleAsync(1, CancellationToken.None);
        
        // Assert
        result.ThisWeek.Should().HaveCount(2);
        foreach (var userOverviewEvent in result.Today)
        {
            Console.WriteLine(userOverviewEvent.Title);
        }
        result.ThisWeek.Should().ContainSingle(e => e.Title == "2");
        result.ThisWeek.Should().ContainSingle(e => e.Title == "5");
    }
    
    [Test]
    public async Task ReturnsOverview_WithExpectedEvents_ForFutureEvents()
    {
        // Arrange
        var user = UserTestData.CreateDefaultUser().ToModel();
        await Db.Users.AddAsync(user);
        
        var oneTimeEvent1 = OneTimeEventTestData.CreateDefault(title: "1", date: dateTimeProviderFake.CurrentDate).ToModel();
        var oneTimeEvent2 = OneTimeEventTestData.CreateDefault(title: "2", date: dateTimeProviderFake.CurrentDate.AddDays(-1)).ToModel();
        var oneTimeEvent3 = OneTimeEventTestData.CreateDefault(title: "3", date: dateTimeProviderFake.CurrentDate.AddDays(8)).ToModel();
        
        var repeatedEvent1 = RepeatedEventTestData
            .CreateDefaultWithDefaultDateTimes(title: "4", firstOccurrence: dateTimeProviderFake.CurrentDate, lastOccurrence: dateTimeProviderFake.CurrentDate.AddDays(7))
            .ToModel();
        var repeatedEvent2 = RepeatedEventTestData
            .CreateDefaultWithDefaultDateTimes(title: "5", firstOccurrence: dateTimeProviderFake.CurrentDate.AddDays(2), lastOccurrence: dateTimeProviderFake.CurrentDate.AddDays(14))
            .ToModel();
        var repeatedEvent3 = RepeatedEventTestData
            .CreateDefaultWithDefaultDateTimes(title: "6", firstOccurrence: dateTimeProviderFake.CurrentDate.AddDays(8), lastOccurrence: dateTimeProviderFake.CurrentDate.AddDays(15))
            .ToModel();
        var repeatedEvent4 = RepeatedEventTestData
            .CreateDefaultWithDefaultDateTimes(title: "7", firstOccurrence: dateTimeProviderFake.CurrentDate.AddDays(-8), lastOccurrence: dateTimeProviderFake.CurrentDate.AddDays(-15))
            .ToModel();

        await Db.AddRangeAsync(oneTimeEvent1, oneTimeEvent2, oneTimeEvent3, repeatedEvent1, repeatedEvent2, repeatedEvent3, repeatedEvent4);
        await Db.SaveChangesAsync();
        
        // Act
        var result = await queryHandler.HandleAsync(1, CancellationToken.None);
        
        // Assert
        result.FutureEvents.Should().HaveCount(2);
        foreach (var userOverviewEvent in result.Today)
        {
            Console.WriteLine(userOverviewEvent.Title);
        }
        result.FutureEvents.Should().ContainSingle(e => e.Title == "3");
        result.FutureEvents.Should().ContainSingle(e => e.Title == "6");
    }
}