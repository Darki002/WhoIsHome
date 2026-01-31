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
        DbMock.Setup(c => c.EventGroups).ReturnsDbSet([]);

        // Act
        var result = await queryHandler.HandleAsync(1, CancellationToken.None);

        // Assert
        result.User.Id.Should().Be(1);
    }

    [Test]
    public async Task ReturnsOverview_WithExpectedEvents()
    {
        // Arrange
        var user = UserTestData.CreateDefaultUser();
        DbMock.Setup(c => c.Users).ReturnsDbSet([user]);
        
        var eventGroup = EventGroupTestData.CreateDefault(endDate: dateTimeProviderFake.CurrentDate.AddDays(8));
        
        var eventInstance1 = EventInstanceTestData.CreateDefault(title: "1", date: dateTimeProviderFake.CurrentDate);
        var eventInstance2 = EventInstanceTestData.CreateDefault(title: "2", date: dateTimeProviderFake.CurrentDate.AddDays(-1));
        var eventInstance3 = EventInstanceTestData.CreateDefault(title: "3", date: dateTimeProviderFake.CurrentDate.AddDays(8));
        
        eventInstance1.EventGroup = eventGroup;
        eventInstance2.EventGroup = eventGroup;
        eventInstance3.EventGroup = eventGroup;
        
        eventGroup.Events = [eventInstance1, eventInstance2, eventInstance3];
        DbMock.Setup(c => c.EventGroups).ReturnsDbSet([eventGroup]);
        
        // Act
        var result = await queryHandler.HandleAsync(1, CancellationToken.None);
        
        // Assert
        result.Events.Should().HaveCount(1);
        result.Events.Should().ContainSingle(e => e.Title == eventGroup.Title);
    }
    
    [Test]
    public async Task ReturnsOverview_OnlyFromEventGroupsThatDidNotEndedYet()
    {
        // Arrange
        var user = UserTestData.CreateDefaultUser();
        DbMock.Setup(c => c.Users).ReturnsDbSet([user]);

        var eventGroup = EventGroupTestData.CreateDefault(endDate: dateTimeProviderFake.CurrentDate.AddDays(1));
        var eventGroup2 = EventGroupTestData.CreateDefault(endDate: dateTimeProviderFake.CurrentDate.AddDays(-1));

        eventGroup.Events = [EventInstanceTestData.CreateDefault()];
        
        DbMock.Setup(c => c.EventGroups).ReturnsDbSet([eventGroup, eventGroup2]);
        
        // Act
        var result = await queryHandler.HandleAsync(1, CancellationToken.None);
        
        // Assert
        result.Events.Should().HaveCount(1);
        result.Events.Should().ContainSingle(e => e.Title == eventGroup.Title);
    }
    
    [Test]
    public async Task ReturnsOverview_WithNextDate_IsExpectedDate()
    {
        // Arrange
        var user = UserTestData.CreateDefaultUser();
        DbMock.Setup(c => c.Users).ReturnsDbSet([user]);
        
        var eventGroup = EventGroupTestData.CreateDefault(endDate: dateTimeProviderFake.CurrentDate.AddDays(8));
        
        var eventInstance1 = EventInstanceTestData.CreateDefault(title: "1", date: dateTimeProviderFake.CurrentDate);
        var eventInstance2 = EventInstanceTestData.CreateDefault(title: "2", date: dateTimeProviderFake.CurrentDate.AddDays(-1));
        var eventInstance3 = EventInstanceTestData.CreateDefault(title: "3", date: dateTimeProviderFake.CurrentDate.AddDays(8));
        
        eventGroup.Events = [eventInstance1, eventInstance2, eventInstance3];
        DbMock.Setup(c => c.EventGroups).ReturnsDbSet([eventGroup]);
        
        // Act
        var result = await queryHandler.HandleAsync(1, CancellationToken.None);
        
        // Assert
        result.Events.Should().HaveCount(1);
        result.Events.Should().ContainSingle(e => e.Title == eventGroup.Title);
        result.Events.Single().NextDate.Should().Be(dateTimeProviderFake.CurrentDate.AddDays(8));
        result.Events.Single().HasRepetitions.Should().BeTrue();
    }
    
    [Test]
    public async Task ReturnsOverview_HasRepetitionIsFalse_WhenStartAndEndDateAreEqual()
    {
        // Arrange
        var user = UserTestData.CreateDefaultUser();
        DbMock.Setup(c => c.Users).ReturnsDbSet([user]);
        
        var eventGroup = EventGroupTestData.CreateDefault(startDate: dateTimeProviderFake.CurrentDate, endDate: dateTimeProviderFake.CurrentDate);
        
        var eventInstance1 = EventInstanceTestData.CreateDefault(title: "1", date: dateTimeProviderFake.CurrentDate);
        
        eventGroup.Events = [eventInstance1];
        DbMock.Setup(c => c.EventGroups).ReturnsDbSet([eventGroup]);
        
        // Act
        var result = await queryHandler.HandleAsync(1, CancellationToken.None);
        
        // Assert
        result.Events.Should().HaveCount(1);
        result.Events.Should().ContainSingle(e => e.Title == eventGroup.Title);
        result.Events.Single().HasRepetitions.Should().BeFalse();
    }
}