using WhoIsHome.Aggregates;
using WhoIsHome.Aggregates.Mappers;
using WhoIsHome.Services;
using WhoIsHome.Shared.Exceptions;
using WhoIsHome.Test.TestData;

namespace WhoIsHome.Test.Application.Services;

[TestFixture]
public class RepeatedEventAggregateServiceTest : InMemoryDbTest
{
    private User user = null!;
    private UserContextFake userContextFake;
    private RepeatedEventAggregateService service;

    [SetUp]
    public void SetUp()
    {
        userContextFake = new UserContextFake();
        userContextFake.SetUser(user, 1);
        service = new RepeatedEventAggregateService(Db, userContextFake);
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
            var repeatedEvent = RepeatedEventTestData.CreateDefault();
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
            var repeatedEvent = RepeatedEventTestData.CreateDefault();
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
            
            var repeatedEvent = RepeatedEventTestData.CreateDefault(userId: 2);
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
            var repeatedEvent = RepeatedEventTestData.CreateDefault(title: "SaveGivenEventToDb");
            
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
            var repeatedEvent = RepeatedEventTestData.CreateDefault(title: "SaveGivenEventToDb");
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
    
    private async Task SaveToDb(RepeatedEvent oneTimeEvent)
    {
        var model = oneTimeEvent.ToModel();
        await Db.RepeatedEvents.AddAsync(model);
        await Db.SaveChangesAsync();
        Db.ChangeTracker.Clear();
    }
}