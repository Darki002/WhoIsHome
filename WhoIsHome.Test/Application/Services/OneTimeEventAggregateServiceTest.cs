using WhoIsHome.Aggregates;
using WhoIsHome.Aggregates.Mappers;
using WhoIsHome.Services;
using WhoIsHome.Shared.Exceptions;
using WhoIsHome.Test.TestData;

namespace WhoIsHome.Test.Application.Services;

[TestFixture]
public class OneTimeEventAggregateServiceTest : InMemoryDbTest
{
    private User user = null!;
    private UserContextFake userContextFake;
    private OneTimeEventAggregateService service;

    [SetUp]
    public void SetUp()
    {
        userContextFake = new UserContextFake();
        userContextFake.SetUser(user, 1);
        service = new OneTimeEventAggregateService(Db, userContextFake);
    }

    protected override async Task DbSetUpAsync()
    {
        user = UserTestData.CreateDefaultUser();
        await Db.Users.AddAsync(user.ToModel());
        await Db.SaveChangesAsync();
    }

    [TestFixture]
    private class GetAsync : OneTimeEventAggregateServiceTest
    {
        [Test]
        public async Task ReturnsEvent_WithTheExpectedId()
        {
            // Arrange
            var oneTimeEvent = OneTimeEventTestData.CreateDefault();
            await SaveToDb(oneTimeEvent);
            
            // Act
            var result = await service.GetAsync(1, CancellationToken.None);
            
            // Assert
            result.Id.Should().Be(1);
            result.Title.Should().Be(oneTimeEvent.Title);
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
    private class DeleteAsync : OneTimeEventAggregateServiceTest
    {
        [Test]
        public async Task DeletesEvent_WithTheGivenId()
        {
            // Arrange
            var oneTimeEvent = OneTimeEventTestData.CreateDefault();
            await SaveToDb(oneTimeEvent);
            
            // Act
            await service.DeleteAsync(1, CancellationToken.None);
            
            // Assert
            Db.OneTimeEvents.Should().HaveCount(0);
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
            
            var oneTimeEvent = OneTimeEventTestData.CreateDefault(userId: 2);
            await SaveToDb(oneTimeEvent);
            
            // Act
            var act = async () => await service.DeleteAsync(1, CancellationToken.None);
            
            // Assert
            await act.Should().ThrowAsync<ActionNotAllowedException>();
        }
    }
    
    [TestFixture]
    private class CreateAsync : OneTimeEventAggregateServiceTest
    {
        [Test]
        public async Task SaveGivenEventToDb()
        {
            // Arrange
            var oneTimeEvent = OneTimeEventTestData.CreateDefault(title: "SaveGivenEventToDb");
            
            // Act
            var result = await service.CreateAsync(oneTimeEvent.Title, oneTimeEvent.Date, oneTimeEvent.StartTime,
                oneTimeEvent.EndTime, oneTimeEvent.DinnerTime.PresenceType, oneTimeEvent.DinnerTime.Time,
                CancellationToken.None);
            
            // Assert
            Db.OneTimeEvents.Should().HaveCount(1);
            Db.OneTimeEvents.Single().Id.Should().Be(1);
            result.Id.Should().Be(1);
            Db.OneTimeEvents.Single().Title.Should().BeEquivalentTo(oneTimeEvent.Title);
        }
    }
    
    [TestFixture]
    private class UpdateAsync : OneTimeEventAggregateServiceTest
    {
        [Test]
        public async Task SaveGivenEventToDb()
        {
            // Arrange
            var oneTimeEvent = OneTimeEventTestData.CreateDefault(title: "SaveGivenEventToDb");
            await SaveToDb(oneTimeEvent);
            
            // Act
            var result = await service.UpdateAsync(1, "This is a new title", oneTimeEvent.Date, oneTimeEvent.StartTime,
                oneTimeEvent.EndTime, oneTimeEvent.DinnerTime.PresenceType, oneTimeEvent.DinnerTime.Time,
                CancellationToken.None);
            
            // Assert
            Db.OneTimeEvents.Should().HaveCount(1);
            Db.OneTimeEvents.Single().Id.Should().Be(1);
            result.Id.Should().Be(1);
            Db.OneTimeEvents.Single().Title.Should().Be("This is a new title");
            result.Title.Should().Be("This is a new title");
        }
    }
    
    private async Task SaveToDb(OneTimeEvent oneTimeEvent)
    {
        var userModel = Db.Users.Single(u => u.Id == oneTimeEvent.UserId);
        var model = oneTimeEvent.ToModel(userModel);
        await Db.OneTimeEvents.AddAsync(model);
        await Db.SaveChangesAsync();
        Db.ChangeTracker.Clear();
    }
}