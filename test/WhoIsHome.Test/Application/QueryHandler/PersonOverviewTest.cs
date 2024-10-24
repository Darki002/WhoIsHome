using WhoIsHome.Aggregates.Mappers;
using WhoIsHome.QueryHandler.PersonOverview;
using WhoIsHome.Shared.Helper;
using WhoIsHome.Test.TestData;

namespace WhoIsHome.Test.Application.QueryHandler;

[TestFixture]
public class PersonOverviewTest : InMemoryDbTest
{
    [SetUp]
    public void SetUp()
    {
        queryHandler = new PersonOverviewQueryHandler(DbFactory);
    }

    private PersonOverviewQueryHandler queryHandler;

    [Test]
    public async Task ReturnsOverview_FromExpectedPerson()
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
        
        var oneTimeEvent1 = OneTimeEventTestData.CreateDefault(title: "1", date: DateOnlyHelper.Today).ToModel();
        var oneTimeEvent2 = OneTimeEventTestData.CreateDefault(title: "2", date: DateOnlyHelper.Today.AddDays(-1)).ToModel();
        var oneTimeEvent3 = OneTimeEventTestData.CreateDefault(title: "3", date: DateOnlyHelper.Today.AddDays(1)).ToModel();
        
        var repeatedEvent1 = RepeatedEventTestData
            .CreateDefault(title: "4", firstOccurrence: DateOnlyHelper.Today, lastOccurrence: DateOnlyHelper.Today.AddDays(7))
            .ToModel();
        var repeatedEvent2 = RepeatedEventTestData
            .CreateDefault(title: "5", firstOccurrence: DateOnlyHelper.Today.AddDays(7), lastOccurrence: DateOnlyHelper.Today.AddDays(14))
            .ToModel();
        var repeatedEvent3 = RepeatedEventTestData
            .CreateDefault(title: "6", firstOccurrence: DateOnlyHelper.Today.AddDays(-14), lastOccurrence: DateOnlyHelper.Today.AddDays(-7))
            .ToModel();

        await Db.AddRangeAsync(oneTimeEvent1, oneTimeEvent2, oneTimeEvent3, repeatedEvent1, repeatedEvent2, repeatedEvent3);
        await Db.SaveChangesAsync();
        
        // Act
        var result = await queryHandler.HandleAsync(1, CancellationToken.None);
        
        // Assert
        result.Today.Should().HaveCount(2);
        foreach (var personOverviewEvent in result.Today)
        {
            Console.WriteLine(personOverviewEvent.Title);
        }
        result.Today.Should().ContainSingle(e => e.Title == "1");
        result.Today.Should().ContainSingle(e => e.Title == "4");
    }
    
    [Test]
    public async Task ReturnsOverview_WithExpectedEvents_ForThisWeek()
    {
        // Arrange
        var user = UserTestData.CreateDefaultUser().ToModel();
        await Db.Users.AddAsync(user);
        
        var oneTimeEvent1 = OneTimeEventTestData.CreateDefault(title: "1", date: DateOnlyHelper.Today).ToModel();
        var oneTimeEvent2 = OneTimeEventTestData.CreateDefault(title: "2", date: DateOnlyHelper.Today.AddDays(1)).ToModel();
        var oneTimeEvent3 = OneTimeEventTestData.CreateDefault(title: "3", date: DateOnlyHelper.Today.AddDays(-1)).ToModel();
        
        var repeatedEvent1 = RepeatedEventTestData
            .CreateDefault(title: "4", firstOccurrence: DateOnlyHelper.Today, lastOccurrence: DateOnlyHelper.Today.AddDays(7))
            .ToModel();
        var repeatedEvent2 = RepeatedEventTestData
            .CreateDefault(title: "5", firstOccurrence: DateOnlyHelper.Today.AddDays(2), lastOccurrence: DateOnlyHelper.Today.AddDays(14))
            .ToModel();
        var repeatedEvent3 = RepeatedEventTestData
            .CreateDefault(title: "6", firstOccurrence: DateOnlyHelper.Today.AddDays(-14), lastOccurrence: DateOnlyHelper.Today.AddDays(-7))
            .ToModel();

        await Db.AddRangeAsync(oneTimeEvent1, oneTimeEvent2, oneTimeEvent3, repeatedEvent1, repeatedEvent2, repeatedEvent3);
        await Db.SaveChangesAsync();
        
        // Act
        var result = await queryHandler.HandleAsync(1, CancellationToken.None);
        
        // Assert
        result.Today.Should().HaveCount(2);
        foreach (var personOverviewEvent in result.Today)
        {
            Console.WriteLine(personOverviewEvent.Title);
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
        
        var oneTimeEvent1 = OneTimeEventTestData.CreateDefault(title: "1", date: DateOnlyHelper.Today).ToModel();
        var oneTimeEvent2 = OneTimeEventTestData.CreateDefault(title: "2", date: DateOnlyHelper.Today.AddDays(-1)).ToModel();
        var oneTimeEvent3 = OneTimeEventTestData.CreateDefault(title: "3", date: DateOnlyHelper.Today.AddDays(8)).ToModel();
        
        var repeatedEvent1 = RepeatedEventTestData
            .CreateDefault(title: "4", firstOccurrence: DateOnlyHelper.Today, lastOccurrence: DateOnlyHelper.Today.AddDays(7))
            .ToModel();
        var repeatedEvent2 = RepeatedEventTestData
            .CreateDefault(title: "5", firstOccurrence: DateOnlyHelper.Today.AddDays(2), lastOccurrence: DateOnlyHelper.Today.AddDays(14))
            .ToModel();
        var repeatedEvent3 = RepeatedEventTestData
            .CreateDefault(title: "6", firstOccurrence: DateOnlyHelper.Today.AddDays(8), lastOccurrence: DateOnlyHelper.Today.AddDays(15))
            .ToModel();
        var repeatedEvent4 = RepeatedEventTestData
            .CreateDefault(title: "7", firstOccurrence: DateOnlyHelper.Today.AddDays(-8), lastOccurrence: DateOnlyHelper.Today.AddDays(-15))
            .ToModel();

        await Db.AddRangeAsync(oneTimeEvent1, oneTimeEvent2, oneTimeEvent3, repeatedEvent1, repeatedEvent2, repeatedEvent3, repeatedEvent4);
        await Db.SaveChangesAsync();
        
        // Act
        var result = await queryHandler.HandleAsync(1, CancellationToken.None);
        
        // Assert
        result.Today.Should().HaveCount(2);
        foreach (var personOverviewEvent in result.Today)
        {
            Console.WriteLine(personOverviewEvent.Title);
        }
        result.FutureEvents.Should().ContainSingle(e => e.Title == "3");
        result.FutureEvents.Should().ContainSingle(e => e.Title == "6");
    }
}