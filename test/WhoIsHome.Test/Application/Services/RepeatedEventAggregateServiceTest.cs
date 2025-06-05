using Moq;
using WhoIsHome.Aggregates;
using WhoIsHome.Aggregates.Mappers;
using WhoIsHome.Handlers;
using WhoIsHome.Services;
using WhoIsHome.Shared.Exceptions;
using WhoIsHome.Test.TestData;

namespace WhoIsHome.Test.Application.Services;

[TestFixture]
public class RepeatedEventAggregateServiceTest : InMemoryDbTest
{
    private readonly UserContextFake userContextFake = new();
    
    private User user = null!;
    private RepeatedEventAggregateService service;

    [SetUp]
    public void SetUp()
    {
        var eventUpdateHandlerMock = Mock.Of<IEventUpdateHandler>();
        
        userContextFake.SetUser(user, 1);
        service = new RepeatedEventAggregateService(DbFactory, eventUpdateHandlerMock, userContextFake);
    }

    protected override async Task DbSetUpAsync()
    {
        user = UserTestData.CreateDefaultUser();
        await Db.Users.AddAsync(user.ToModel());
        await Db.SaveChangesAsync();
        Db.ChangeTracker.Clear();
    }

    [TestFixture]
    private class GetAsync : RepeatedEventAggregateServiceTest
    {
        [Test]
        public async Task ReturnsEvent_WithTheExpectedId()
        {
            // Arrange
            var repeatedEvent = RepeatedEventTestData.CreateDefaultWithDefaultDateTimes();
            await SaveToDb(repeatedEvent);
            
            // Act
            var result = await service.GetAsync(1, CancellationToken.None);
            
            // Assert
            result.Id.Should().Be(1);
            result.Title.Should().Be(repeatedEvent.Title);
        }
        
        [Test]
        public async Task ThrowsNotFoundException_WhenNoEventWithTheGivenIdWasFound()
        {
            // Act
            var act = async () => await service.GetAsync(1, CancellationToken.None);
            
            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
    
    [TestFixture]
    private class DeleteAsync : RepeatedEventAggregateServiceTest
    {
        [Test]
        public async Task DeletesEvent_WithTheGivenId()
        {
            // Arrange
            var repeatedEvent = RepeatedEventTestData.CreateDefaultWithDefaultDateTimes();
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
            
            var repeatedEvent = RepeatedEventTestData.CreateDefaultWithDefaultDateTimes(userId: 2);
            await SaveToDb(repeatedEvent);
            
            // Act
            var act = async () => await service.DeleteAsync(1, CancellationToken.None);
            
            // Assert
            await act.Should().ThrowAsync<ActionNotAllowedException>();
        }
    }
    
    [TestFixture]
    private class CreateAsync : RepeatedEventAggregateServiceTest
    {
        [Test]
        public async Task SaveGivenEventToDb()
        {
            // Arrange
            var repeatedEvent = RepeatedEventTestData.CreateDefaultWithDefaultDateTimes(title: "SaveGivenEventToDb");
            
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
    private class UpdateAsync : RepeatedEventAggregateServiceTest
    {
        [Test]
        public async Task SaveGivenEventToDb()
        {
            // Arrange
            var repeatedEvent = RepeatedEventTestData.CreateDefaultWithDefaultDateTimes(title: "SaveGivenEventToDb");
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
    private class EndAsync : RepeatedEventAggregateServiceTest
    {
        [Test]
        public async Task SetEndTimeOnAggregate()
        {
            // Arrange
            var expected = new DateOnly(2025, 1, 1);
            
            var repeatedEvent = RepeatedEventTestData.CreateDefault(title: "SetEndTimeOnAggregate");
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
            var repeatedEvent = RepeatedEventTestData.CreateDefault(title: "SetEndTimeOnAggregate", lastOccurrence: expected);
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
            var repeatedEvent = RepeatedEventTestData.CreateDefault(title: "SetEndTimeOnAggregate", firstOccurrence: expected.AddDays(7), lastOccurrence: expected);
            await SaveToDb(repeatedEvent);
            
            // Act
            var action = async () => await service.EndAsync(1, expected, CancellationToken.None);
            
            // Assert
            await action.Should().ThrowAsync<InvalidModelException>();
        }
    }
    
    private async Task SaveToDb(RepeatedEvent oneTimeEvent)
    {
        var model = oneTimeEvent.ToModel();
        await Db.RepeatedEvents.AddAsync(model);
        await Db.SaveChangesAsync();
        Db.ChangeTracker.Clear();
    }
}