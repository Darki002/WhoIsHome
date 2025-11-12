using Moq;
using Moq.EntityFrameworkCore;
using WhoIsHome.Entities;
using WhoIsHome.External.Database;
using WhoIsHome.Handlers;
using WhoIsHome.Services;
using WhoIsHome.Shared.Types;
using WhoIsHome.Test.TestData;

namespace WhoIsHome.Test.Application.Services;

[TestFixture]
public class EventServiceTest : DbMockTest
{
    private User user = null!;
    private DateTimeProviderFake dateTimeProvider;
    private Mock<IEventUpdateHandler> eventUpdateHandlerMock;
    private EventService service;

    [SetUp]
    public void SetUp()
    {
        eventUpdateHandlerMock = new Mock<IEventUpdateHandler>();
        dateTimeProvider = new DateTimeProviderFake();
        service = new EventService(Db, eventUpdateHandlerMock.Object, dateTimeProvider);
    }

    protected override void DbSetUp(Mock<WhoIsHomeContext> mock)
    {
        user = UserTestData.CreateDefaultUser();
        mock.Setup(c => c.Users).ReturnsDbSet([user]);
    }

    [TestFixture]
    private class GenerateNewAsync : EventServiceTest
    {
        [Test]
        public async Task GeneratesExpectedEvents_FromGivenEventGroup()
        {
            // Arrange
            const WeekDay weekDays = WeekDay.Monday | WeekDay.Friday;
            var eventGroup = EventGroupTestData.CreateDefault(
                startDate: dateTimeProvider.CurrentDate, 
                endDate: dateTimeProvider.CurrentDate.AddDays(14), 
                weekDays: weekDays);
            
            List<EventInstance> result = [];
            
            DbMock.Setup(c =>
                    c.EventInstances.AddRangeAsync(It.IsAny<List<EventInstance>>(), It.IsAny<CancellationToken>()))
                .Callback<List<EventInstance>>(r => result = r);
            
            // Act
            await service.GenerateNewAsync(eventGroup, CancellationToken.None);
            
            // Assert
            result.Should().HaveCount(4);
            result.Should().AllSatisfy(i => i.Date.Should().Be(i.OriginalDate));
            result[0].Date.Should().Be(new DateOnly(2024, 11, 29));
            result[1].Date.Should().Be(new DateOnly(2024, 12, 2));
            result[2].Date.Should().Be(new DateOnly(2024, 12, 6));
            result[3].Date.Should().Be(new DateOnly(2024, 12, 9));
            eventUpdateHandlerMock.Verify(
                void (x) => x.HandleAsync(
                    It.IsAny<EventInstance>(), 
                    It.IsAny<EventUpdateHandler.UpdateAction>())
                , Times.Never);
        }
        
        [Test]
        public async Task TriggersEventUpdateHandler_WhenEventIsGeneratedToday()
        {
            // Arrange
            const WeekDay weekDays = WeekDay.Tuesday;
            var eventGroup = EventGroupTestData.CreateDefault(
                startDate: dateTimeProvider.CurrentDate, 
                endDate: dateTimeProvider.CurrentDate.AddDays(3),
                weekDays: weekDays);
            
            // Act
            await service.GenerateNewAsync(eventGroup, CancellationToken.None);
            
            // Assert
            eventUpdateHandlerMock.Verify(
                void (x) => x.HandleAsync(
                    It.Is<EventInstance>(e => e.Date == dateTimeProvider.CurrentDate), 
                    It.Is<EventUpdateHandler.UpdateAction>(a => a == EventUpdateHandler.UpdateAction.Create))
                , Times.Exactly(1));
        }
    }
    
    [TestFixture]
    private class GenerateUpdateAsync : EventServiceTest
    {
        [Test]
        public async Task GeneratesExpectedUpdate_FromGivenEventGroup()
        {
            // Arrange
            const WeekDay weekDays = WeekDay.Wednesday | WeekDay.Thursday;
            var eventGroup = EventGroupTestData.CreateDefault(
                startDate: dateTimeProvider.CurrentDate, 
                endDate: dateTimeProvider.CurrentDate.AddDays(4),
                weekDays: weekDays);
            
            List<EventInstance> result = [];
            List<EventInstance> deletes = [];

            var event1 = EventInstanceTestData.CreateDefault(date: new DateOnly(2024, 11, 25));
            var event2 = EventInstanceTestData.CreateDefault(date: new DateOnly(2024, 11, 26));

            DbMock.Setup(c => c.EventInstances).ReturnsDbSet([event1, event2]);
            DbMock.Setup(c =>
                    c.EventInstances.AddRangeAsync(It.IsAny<List<EventInstance>>(), It.IsAny<CancellationToken>()))
                .Callback<List<EventInstance>>(r => result = r);
            DbMock.Setup(c =>
                    c.EventInstances.RemoveRange(It.IsAny<EventInstance[]>()))
                .Callback<EventInstance[]>(r => deletes = r.ToList());
            
            // Act
            await service.GenerateUpdateAsync(eventGroup, CancellationToken.None);
            
            // Assert
            deletes.Should().HaveCount(2);
            result.Should().HaveCount(2);
            result.Should().AllSatisfy(i => i.Date.Should().Be(i.OriginalDate));
            result[0].Date.Should().Be(new DateOnly(2024, 11, 27));
            result[1].Date.Should().Be(new DateOnly(2024, 11, 28));
            eventUpdateHandlerMock.Verify(
                void (x) => x.HandleAsync(
                    It.IsAny<EventInstance>(), 
                    It.IsAny<EventUpdateHandler.UpdateAction>())
                , Times.Never);
        }
        
        [Test]
        public async Task DoesNotDeleteEditedInstances_AndRegeneratesOriginals()
        {
            // Arrange
            const WeekDay weekDays = WeekDay.Wednesday | WeekDay.Friday;
            var eventGroup = EventGroupTestData.CreateDefault(
                startDate: dateTimeProvider.CurrentDate, 
                endDate: dateTimeProvider.CurrentDate.AddDays(4),
                weekDays: weekDays);
            
            List<EventInstance> result = [];
            List<EventInstance> deletes = [];

            var event1 = EventInstanceTestData.CreateDefault(
                date: new DateOnly(2024, 11, 27), 
                originalDate: new DateOnly(2024, 11, 28), 
                dinnerTime: new TimeOnly(19, 00), 
                isOriginal: false);

            var date = new DateOnly(2024, 11, 29);
            var event2 = EventInstanceTestData.CreateDefault(date: date);

            DbMock.Setup(c => c.EventInstances).ReturnsDbSet([event1, event2]);
            DbMock.Setup(c =>
                    c.EventInstances.AddRangeAsync(It.IsAny<List<EventInstance>>(), It.IsAny<CancellationToken>()))
                .Callback<List<EventInstance>>(r => result = r);
            DbMock.Setup(c =>
                    c.EventInstances.RemoveRange(It.IsAny<EventInstance[]>()))
                .Callback<EventInstance[]>(r => deletes = r.ToList());
            
            // Act
            await service.GenerateUpdateAsync(eventGroup, CancellationToken.None);
            
            // Assert
            deletes.Should().HaveCount(1);
            deletes.Single().Date.Should().Be(date);
            result.Should().HaveCount(1);
            result.Single().Date.Should().Be(date);
        }
        
        [Test]
        public async Task UpdatesTitleOfNonOriginalInstances()
        {
            // Arrange
            const string newTitle = "new";
            const WeekDay weekDays = WeekDay.Wednesday;
            var eventGroup = EventGroupTestData.CreateDefault(
                startDate: dateTimeProvider.CurrentDate,
                endDate: dateTimeProvider.CurrentDate.AddDays(4),
                weekDays: weekDays);

            var eventInstance = EventInstanceTestData.CreateDefault(
                title: "false",
                date: new DateOnly(2024, 11, 27), 
                originalDate: new DateOnly(2024, 11, 28), 
                dinnerTime: new TimeOnly(19, 00), 
                isOriginal: false);

            DbMock.Setup(c => c.EventInstances).ReturnsDbSet([eventInstance]);
            
            List<EventInstance> result = [];
            DbMock.Setup(c =>
                    c.EventInstances.UpdateRange(It.IsAny<EventInstance[]>()))
                .Callback<EventInstance[]>(r => result = r.ToList());
            
            // Act
            await service.GenerateUpdateAsync(eventGroup, CancellationToken.None);
            
            // Assert
            result.Should().HaveCount(1);
            result.Single().Title.Should().Be(newTitle);
        }
        
        [Test]
        public async Task TriggersEventUpdateHandler_WhenEventIsGeneratedToday()
        {
            // Arrange
            const WeekDay weekDays = WeekDay.Tuesday;
            var eventGroup = EventGroupTestData.CreateDefault(
                startDate: dateTimeProvider.CurrentDate, 
                endDate: dateTimeProvider.CurrentDate.AddDays(4),
                weekDays: weekDays);
            
            DbMock.Setup(c => c.EventInstances).ReturnsDbSet([]);
            
            // Act
            await service.GenerateUpdateAsync(eventGroup, CancellationToken.None);
            
            // Assert
            eventUpdateHandlerMock.Verify(
                void (x) => x.HandleAsync(
                    It.Is<EventInstance>(e => e.Date == dateTimeProvider.CurrentDate), 
                    It.Is<EventUpdateHandler.UpdateAction>(a => a == EventUpdateHandler.UpdateAction.Update))
                , Times.Exactly(1));
        }
    }

    [TestFixture]
    private class DeleteAsync : EventServiceTest
    {
        [Test]
        public async Task DeletesAllOriginalDates_FromGivenEventGroup()
        {
            // Arrange
            var event1 = EventInstanceTestData.CreateDefault(isOriginal: false);
            var date = new DateOnly(2024, 11, 29);
            var event2 = EventInstanceTestData.CreateDefault(date: date);

            DbMock.Setup(c => c.EventInstances).ReturnsDbSet([event1, event2]);
            
            List<EventInstance> deletes = [];
            DbMock.Setup(c =>
                    c.EventInstances.RemoveRange(It.IsAny<EventInstance[]>()))
                .Callback<EventInstance[]>(r => deletes = r.ToList());
            
            // Act
            await service.DeleteAsync(1);
            
            // Assert
            deletes.Should().HaveCount(1);
            deletes.Single().Date.Should().Be(date);
        }
        
        [Test]
        public async Task TriggersEventUpdateHandler_WhenDeletingEventFromToday()
        {
            // Arrange
            var eventInstance = EventInstanceTestData.CreateDefault(date: dateTimeProvider.CurrentDate);
            DbMock.Setup(c => c.EventInstances).ReturnsDbSet([eventInstance]);
            
            // Act
            await service.DeleteAsync(1);
            
            // Assert
            eventUpdateHandlerMock.Verify(
                void (x) => x.HandleAsync(
                    It.Is<EventInstance>(e => e.Date == dateTimeProvider.CurrentDate), 
                    It.Is<EventUpdateHandler.UpdateAction>(a => a == EventUpdateHandler.UpdateAction.Delete))
                , Times.Exactly(1));
        }
    }

    [TestFixture]
    private class EditSingleInstanceAsync : EventServiceTest
    {
        [Test]
        public async Task DeletesSingleEventInstance()
        {
            // Arrange
            var date = new DateOnly(2024, 11, 29);
            var event1 = EventInstanceTestData.CreateDefault(id: 1, date: new DateOnly(2024, 11, 28));
            var event2 = EventInstanceTestData.CreateDefault(id: 2, date: date);

            DbMock.Setup(c => c.EventInstances).ReturnsDbSet([event1, event2]);
            
            EventInstance? editedEvent = null;
            DbMock.Setup(c =>
                    c.EventInstances.Update(It.IsAny<EventInstance>()))
                .Callback<EventInstance>(r => editedEvent = r);

            var newDate = new DateOnly(2024, 11, 30);
            var newStartTime = new TimeOnly(18, 00);
            var newEndTime = new TimeOnly(19, 00);
            var newPresenceType = PresenceType.Late;
            var newDinnerTime = new TimeOnly(20, 00);
            
            // Act
            await service.EditSingleInstanceAsync(
                originalDate: date,
                date: newDate,
                startTime: newStartTime,
                endTime: newEndTime,
                presenceType: newPresenceType,
                dinnerTime: newDinnerTime);
            
            // Assert
            editedEvent.Should().NotBeNull();
            editedEvent!.Id.Should().Be(2);
            editedEvent.OriginalDate.Should().Be(date);
            editedEvent.IsOriginal.Should().BeFalse();
            editedEvent.Date.Should().Be(newDate);
            editedEvent.StartTime.Should().Be(newStartTime);
            editedEvent.EndTime.Should().Be(newEndTime);
            editedEvent.PresenceType.Should().Be(newPresenceType);
            editedEvent.DinnerTime.Should().Be(newDinnerTime);
        }
    }
    
    [TestFixture]
    private class DeleteSingleInstanceAsync : EventServiceTest
    {
        [Test]
        public async Task DeletesSingleEventInstance()
        {
            // Arrange
            var date = new DateOnly(2024, 11, 29);
            var event1 = EventInstanceTestData.CreateDefault(id: 1, date: new DateOnly(2024, 11, 28));
            var event2 = EventInstanceTestData.CreateDefault(id: 2, date: date);

            DbMock.Setup(c => c.EventInstances).ReturnsDbSet([event1, event2]);
            
            EventInstance? deletedEvent = null;
            DbMock.Setup(c =>
                    c.EventInstances.Remove(It.IsAny<EventInstance>()))
                .Callback<EventInstance>(r => deletedEvent = r);
            
            // Act
            await service.DeleteSingleInstanceAsync(date);
            
            // Assert
            deletedEvent.Should().NotBeNull();
            deletedEvent!.Id.Should().Be(2);
        }
    }
}