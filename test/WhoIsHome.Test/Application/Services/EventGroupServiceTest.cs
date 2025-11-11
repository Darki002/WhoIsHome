using Moq;
using Moq.EntityFrameworkCore;
using WhoIsHome.Entities;
using WhoIsHome.External.Database;
using WhoIsHome.Handlers;
using WhoIsHome.Services;
using WhoIsHome.Test.Shared.Helper;
using WhoIsHome.Test.TestData;

namespace WhoIsHome.Test.Application.Services;

[TestFixture]
public class EventGroupServiceTest : DbMockTest
{
    private readonly UserContextFake userContextFake = new UserContextFake();
    
    private User user = null!;
    private EventGroupService service;

    [SetUp]
    public void SetUp()
    {
        var eventServiceMock = Mock.Of<IEventService>();
        
        userContextFake.SetUser(user, 1);
        service = new EventGroupService(Db, eventServiceMock, userContextFake);
    }

    protected override void DbSetUp(Mock<WhoIsHomeContext> mock)
    {
        user = UserTestData.CreateDefaultUser();
        mock.Setup(c => c.Users).ReturnsDbSet([user]);
    }

    [TestFixture]
    private class GetAsync : EventGroupServiceTest
    {
        [Test]
        public async Task ReturnsEvent_WithTheExpectedId()
        {
            // Arrange
            var eventGroup = EventGroupTestData.CreateDefaultWithDefaultDateTimes();
            DbMock.Setup(c => c.EventGroups).ReturnsDbSet([eventGroup]);
            
            // Act
            var result = await service.GetAsync(1, CancellationToken.None);
            
            // Assert
            result.Value.Id.Should().Be(1);
            result.Value.Title.Should().Be(eventGroup.Title);
        }
        
        [Test]
        public async Task ThrowsNotFoundException_WhenNoEventWithTheGivenIdWasFound()
        {
            // Act
            var result = await service.GetAsync(1, CancellationToken.None);
            
            // Assert
            result.HasError.Should().BeTrue();
            result.Error.Should().NotBeNull();
        }
    }
    
    [TestFixture]
    private class DeleteAsync : EventGroupServiceTest
    {
        [Test]
        public async Task DeletesEvent_WithTheGivenId()
        {
            // Arrange
            var repeatedEvent = EventGroupTestData.CreateDefaultWithDefaultDateTimes();
            await SaveToDb(repeatedEvent);
            
            // Act
            await service.DeleteAsync(1, CancellationToken.None);
            
            // Assert
            Db.RepeatedEvents.Should().HaveCount(0);
        }
        
        [Test]
        public async Task ThrowsNotFoundException_WhenNoEventWithTheGivenIdWasFound()
        {
            // Act
            var act = async () => await service.DeleteAsync(1, CancellationToken.None);
            
            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
        
        [Test]
        public async Task ThrowsActionNotAllowedException_WhenUserIdDoesNotMatch()
        {
            // Arrange
            var newUser = UserTestData.CreateDefaultUser().ToModel();
            await Db.Users.AddAsync(newUser);
            await Db.SaveChangesAsync();
            
            var repeatedEvent = EventGroupTestData.CreateDefaultWithDefaultDateTimes(userId: 2);
            await SaveToDb(repeatedEvent);
            
            // Act
            var act = async () => await service.DeleteAsync(1, CancellationToken.None);
            
            // Assert
            await act.Should().ThrowAsync<ActionNotAllowedException>();
        }
    }
    
    [TestFixture]
    private class CreateAsync : EventGroupServiceTest
    {
        [Test]
        public async Task SaveGivenEventToDb()
        {
            // Arrange
            var repeatedEvent = EventGroupTestData.CreateDefaultWithDefaultDateTimes(title: "SaveGivenEventToDb");
            
            // Act
            var result = await service.CreateAsync(repeatedEvent.Title, repeatedEvent.FirstOccurrence, repeatedEvent.LastOccurrence, repeatedEvent.StartTime,
                repeatedEvent.EndTime, repeatedEvent.DinnerTime.PresenceType, repeatedEvent.DinnerTime.Time,
                CancellationToken.None);
            
            // Assert
            Db.RepeatedEvents.Should().HaveCount(1);
            Db.RepeatedEvents.Single().Id.Should().Be(1);
            result.Id.Should().Be(1);
            Db.RepeatedEvents.Single().Title.Should().BeEquivalentTo(repeatedEvent.Title);
        }
    }
    
    [TestFixture]
    private class UpdateAsync : EventGroupServiceTest
    {
        [Test]
        public async Task SaveGivenEventToDb()
        {
            // Arrange
            var repeatedEvent = EventGroupTestData.CreateDefaultWithDefaultDateTimes(title: "SaveGivenEventToDb");
            await SaveToDb(repeatedEvent);
            
            // Act
            var result = await service.UpdateAsync(1, "This is a new title", repeatedEvent.FirstOccurrence, repeatedEvent.LastOccurrence, repeatedEvent.StartTime,
                repeatedEvent.EndTime, repeatedEvent.DinnerTime.PresenceType, repeatedEvent.DinnerTime.Time,
                CancellationToken.None);
            
            // Assert
            Db.RepeatedEvents.Should().HaveCount(1);
            Db.RepeatedEvents.Single().Id.Should().Be(1);
            result.Id.Should().Be(1);
            Db.RepeatedEvents.Single().Title.Should().Be("This is a new title");
            result.Title.Should().Be("This is a new title");
        }
    }
    
    [TestFixture]
    private class EndAsync : EventGroupServiceTest
    {
        [Test]
        public async Task SetEndTimeOnAggregate()
        {
            // Arrange
            var expected = new DateOnly(2025, 1, 1);
            
            var repeatedEvent = EventGroupTestData.CreateDefault(title: "SetEndTimeOnAggregate");
            await SaveToDb(repeatedEvent);
            
            // Act
            var result = await service.EndAsync(1, expected, CancellationToken.None);
            
            // Assert
            result.Id.Should().Be(1);
            result.LastOccurrence.Should().Be(expected);
        }
        
        [Test]
        public async Task ThrowIfLastOccurrenceIsAlreadySet()
        {
            // Arrange
            var expected = new DateOnly(2025, 1, 1);
            var repeatedEvent = EventGroupTestData.CreateDefault(title: "SetEndTimeOnAggregate", endDate: expected);
            await SaveToDb(repeatedEvent);
            
            // Act
            var action = async () => await service.EndAsync(1, expected, CancellationToken.None);
            
            // Assert
            await action.Should().ThrowAsync<InvalidModelException>();
        }
        
        [Test]
        public async Task ThrowIfLastOccurrenceIsBeforeFirstOccurrence()
        {
            // Arrange
            var expected = new DateOnly(2025, 1, 1);
            var repeatedEvent = EventGroupTestData.CreateDefault(title: "SetEndTimeOnAggregate", startDate: expected.AddDays(7), endDate: expected);
            await SaveToDb(repeatedEvent);
            
            // Act
            var action = async () => await service.EndAsync(1, expected, CancellationToken.None);
            
            // Assert
            await action.Should().ThrowAsync<InvalidModelException>();
        }
    }
}