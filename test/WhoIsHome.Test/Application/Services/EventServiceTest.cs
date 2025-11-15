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
                    c.EventInstances.AddRangeAsync(
                        It.IsAny<IEnumerable<EventInstance>>(), 
                        It.IsAny<CancellationToken>()))
                .Callback<IEnumerable<EventInstance>, CancellationToken>((r, _) => result = r.ToList());
            
            // Act
            await service.GenerateNewAsync(eventGroup);
            
            // Assert
            result.Should().HaveCount(4);
            result.Should().AllSatisfy(i => i.Date.Should().Be(i.OriginalDate));
            result.Should().AllSatisfy(i => i.IsOriginal.Should().BeTrue());
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

            DbMock.Setup(c => c.EventInstances).ReturnsDbSet([]);
            
            // Act
            await service.GenerateNewAsync(eventGroup);
            
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
                    c.EventInstances.AddRangeAsync(It.IsAny<IEnumerable<EventInstance>>(), It.IsAny<CancellationToken>()))
                .Callback<IEnumerable<EventInstance>, CancellationToken>((r, _) => result = r.ToList());
            DbMock.Setup(c =>
                    c.EventInstances.RemoveRange(It.IsAny<IEnumerable<EventInstance>>()))
                .Callback<IEnumerable<EventInstance>>(r => deletes = r.ToList());
            
            // Act
            await service.GenerateUpdateAsync(eventGroup);
            
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
                date: new DateOnly(2024, 11, 28), 
                originalDate: new DateOnly(2024, 11, 27), 
                dinnerTime: new TimeOnly(19, 00), 
                isOriginal: false);

            var date = new DateOnly(2024, 11, 29);
            var event2 = EventInstanceTestData.CreateDefault(date: date);

            DbMock.Setup(c => c.EventInstances).ReturnsDbSet([event1, event2]);
            DbMock.Setup(c =>
                    c.EventInstances.AddRangeAsync(It.IsAny<IEnumerable<EventInstance>>(), It.IsAny<CancellationToken>()))
                .Callback<IEnumerable<EventInstance>, CancellationToken>((r, _) => result = r.ToList());
            DbMock.Setup(c =>
                    c.EventInstances.RemoveRange(It.IsAny<IEnumerable<EventInstance>>()))
                .Callback<IEnumerable<EventInstance>>(r => deletes = r.ToList());
            
            // Act
            await service.GenerateUpdateAsync(eventGroup);
            
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
                title: newTitle,
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
                    c.EventInstances.AddRangeAsync(It.IsAny<IEnumerable<EventInstance>>(), It.IsAny<CancellationToken>()))
                .Callback<IEnumerable<EventInstance>, CancellationToken>((r, _) => result = r.ToList());
            
            // Act
            await service.GenerateUpdateAsync(eventGroup);
            
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
            await service.GenerateUpdateAsync(eventGroup);
            
            // Assert
            eventUpdateHandlerMock.Verify(
                void (x) => x.HandleAsync(
                    It.Is<EventInstance>(e => e.Date == dateTimeProvider.CurrentDate), 
                    It.Is<EventUpdateHandler.UpdateAction>(a => a == EventUpdateHandler.UpdateAction.Update))
                , Times.Exactly(1));
        }
    }

    [TestFixture]
    private class GenerateNextAsync : EventServiceTest
    {
        [Test]
        public async Task GeneratesExpectedEventInstances_ForGivenEventGroup()
        {
            // Arrange
            const WeekDay weekDays = WeekDay.Wednesday | WeekDay.Thursday;
            var eventGroup = EventGroupTestData.CreateDefault(
                startDate: dateTimeProvider.CurrentDate.AddDays(-7), 
                endDate: dateTimeProvider.CurrentDate.AddDays(14), 
                weekDays: weekDays);

            var date1 = new DateOnly(2024, 11, 20);
            var date2 = new DateOnly(2024, 11, 21);
            var date3 = new DateOnly(2024, 11, 27);
            var date4 = new DateOnly(2024, 11, 28);

            var event1 = EventInstanceTestData.CreateDefault(date: date1);
            var event2 = EventInstanceTestData.CreateDefault(date: date2);
            var event3 = EventInstanceTestData.CreateDefault(date: date3.AddDays(-1), isOriginal: false, originalDate: date3);
            var event4 = EventInstanceTestData.CreateDefault(date: date4);
            DbMock.Setup(c => c.EventInstances).ReturnsDbSet([event1, event2, event3, event4]);
            
            var expected1 = new DateOnly(2024, 12, 4);
            var expected2 = new DateOnly(2024, 12, 5);
            
            List<EventInstance> result = [];
            DbMock.Setup(c =>
                    c.EventInstances.AddRangeAsync(
                        It.IsAny<IEnumerable<EventInstance>>(), 
                        It.IsAny<CancellationToken>()))
                .Callback<IEnumerable<EventInstance>, CancellationToken>((r, _) => result = r.ToList());
            
            // Act
            await service.GenerateNextAsync(eventGroup, CancellationToken.None);
            
            // Assert
            result.Should().HaveCount(2);
            result.Should().AllSatisfy(i => i.Date.Should().Be(i.OriginalDate));
            result.Should().AllSatisfy(i => i.IsOriginal.Should().BeTrue());
            result[0].Date.Should().Be(expected1);
            result[1].Date.Should().Be(expected2);
        }
    }

    [TestFixture]
    private class DeleteAsync : EventServiceTest
    {
        [Test]
        public async Task DeletesAllOriginalDates_FromGivenEventGroup()
        {
            // Arrange
            var event1 = EventInstanceTestData.CreateDefault(isOriginal: false, eventGroupId: 1);
            var date = new DateOnly(2024, 11, 29);
            var event2 = EventInstanceTestData.CreateDefault(date: date, eventGroupId: 1);

            DbMock.Setup(c => c.EventInstances).ReturnsDbSet([event1, event2]);
            
            List<EventInstance> deletes = [];
            DbMock.Setup(c =>
                    c.EventInstances.RemoveRange(It.IsAny<IEnumerable<EventInstance>>()))
                .Callback<IEnumerable<EventInstance>>(r => deletes = r.ToList());
            
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
}