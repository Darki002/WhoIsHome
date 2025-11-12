using Moq;
using Moq.EntityFrameworkCore;
using WhoIsHome.Entities;
using WhoIsHome.External.Database;
using WhoIsHome.Services;
using WhoIsHome.Test.TestData;

namespace WhoIsHome.Test.Application.Services;

[TestFixture]
public class EventGroupServiceTest : DbMockTest
{
    private readonly UserContextFake userContextFake = new UserContextFake();
    
    private User user = null!;
    private EventGroupService service;
    private Mock<IEventService> eventServiceMock;

    [SetUp]
    public void SetUp()
    {
        eventServiceMock = new Mock<IEventService>();
        
        userContextFake.SetUser(user, 1);
        service = new EventGroupService(Db, eventServiceMock.Object, userContextFake);
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
            var eventGroup = EventGroupTestData.CreateDefaultWithDefaultDateTimes();
            DbMock.Setup(c => c.EventGroups).ReturnsDbSet([eventGroup]);
            
            // Act
            var result = await service.DeleteAsync(1, CancellationToken.None);
            
            // Assert
            DbMock.VerifyRemove(c => c.Remove(eventGroup));
            eventServiceMock.Verify(c => c.DeleteAsync(eventGroup.Id));
            result.Should().BeNull();
        }
        
        [Test]
        public async Task ReturnsError_WhenNoEventWithTheGivenIdWasFound()
        {
            // Arrange
            DbMock.Setup(c => c.EventGroups).ReturnsDbSet([]);
            
            // Act
            var result = await service.DeleteAsync(1, CancellationToken.None);
            
            // Assert
            result.Should().NotBeNull();
        }
        
        [Test]
        public async Task ThrowsActionNotAllowedException_WhenUserIdDoesNotMatch()
        {
            // Arrange
            var eventGroup = EventGroupTestData.CreateDefaultWithDefaultDateTimes(userId: 2);
            DbMock.Setup(c => c.EventGroups).ReturnsDbSet([eventGroup]);
            
            // Act
            var result = await service.DeleteAsync(1, CancellationToken.None);
            
            // Assert
            result.Should().NotBeNull();
        }
    }
    
    [TestFixture]
    private class CreateAsync : EventGroupServiceTest
    {
        [Test]
        public async Task SaveGivenEventToDb()
        {
            // Arrange
            const string title = "SaveGivenEventToDb";
            var eventGroup = EventGroupTestData.CreateDefaultWithDefaultDateTimes(title: title);
            
            // Act
            var result = await service.CreateAsync(
                eventGroup.Title, 
                eventGroup.StartDate, 
                eventGroup.EndDate, 
                eventGroup.WeekDays, 
                eventGroup.StartTime,
                eventGroup.EndTime, 
                eventGroup.PresenceType, 
                eventGroup.DinnerTime,
                CancellationToken.None);
            
            // Assert
            result.Title.Should().Be(title);
            DbMock.VerifyAdd(c => c.AddAsync(eventGroup));
        }
    }
    
    [TestFixture]
    private class UpdateAsync : EventGroupServiceTest
    {
        [Test]
        public async Task SaveGivenEventToDb()
        {
            // Arrange
            const string title = "SaveGivenEventToDb";
            var eventGroup = EventGroupTestData.CreateDefaultWithDefaultDateTimes(title: "old-title");
            DbMock.Setup(c => c.EventGroups).ReturnsDbSet([eventGroup]);
            
            // Act
            var result = await service.UpdateAsync(
                id: eventGroup.Id,
                title,
                eventGroup.StartDate, 
                eventGroup.EndDate, 
                eventGroup.WeekDays, 
                eventGroup.StartTime,
                eventGroup.EndTime, 
                eventGroup.PresenceType, 
                eventGroup.DinnerTime,
                CancellationToken.None);
            
            // Assert
            result.HasErrors.Should().BeFalse();
            result.Value.Should().NotBeNull();
            result.Result.Title.Should().Be(title);
            DbMock.Verify(c => c.Update(It.Is<EventGroup>(e => e.Title == title)));
        }
        
        [Test]
        public async Task ReturnsError_WhenEventGroupDoesNotExist()
        {
            // Arrange
            const string title = "SaveGivenEventToDb";
            var eventGroup = EventGroupTestData.CreateDefaultWithDefaultDateTimes(title: "old-title");
            
            // Act
            var result = await service.UpdateAsync(
                id: 2,
                title,
                eventGroup.StartDate, 
                eventGroup.EndDate, 
                eventGroup.WeekDays, 
                eventGroup.StartTime,
                eventGroup.EndTime, 
                eventGroup.PresenceType, 
                eventGroup.DinnerTime,
                CancellationToken.None);
            
            // Assert
            result.HasErrors.Should().BeTrue();
            result.Value.Should().BeNull();
            result.ValidationErrors.Should().HaveCountGreaterThan(0);
        }
    }
}