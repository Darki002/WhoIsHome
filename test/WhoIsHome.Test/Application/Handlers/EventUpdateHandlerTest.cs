using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq.EntityFrameworkCore;
using WhoIsHome.Entities;
using WhoIsHome.Handlers;
using WhoIsHome.Test.TestData;

namespace WhoIsHome.Test.Application.Handlers;

public class EventUpdateHandlerTest : DbMockTest
{
    private readonly DateTimeProviderFake dateTimeProviderFake = new();

    private readonly ILogger<EventUpdateHandler> logger = NullLogger<EventUpdateHandler>.Instance;

    private readonly List<User> userModels =
    [
        new() { Id = 1, UserName = "Darki", Email = "", Password = "" },
        new() { Id = 2, UserName = "Test", Email = "", Password = "" },
        new() { Id = 3, UserName = "Test2", Email = "", Password = "" }
    ];

    [Test]
    public async Task CallsPusUpClient_WithExpectedCommand_Create()
    {
        // Arrange
        var pushUpClientFake = new PushUpContextFake();
        var backgroundTaskQueueFake = new BackgroundTaskQueueFake();
        
        var updatedEvent = EventInstanceTestData.CreateDefault(id: 1, userId: 1, date: dateTimeProviderFake.CurrentDate);
        
        DbMock.Setup(c => c.EventInstances).ReturnsDbSet([updatedEvent]);
        DbMock.Setup(c => c.Users).ReturnsDbSet(userModels);
        var handler = new EventUpdateHandler(Db, pushUpClientFake, dateTimeProviderFake, backgroundTaskQueueFake, logger);

        // Act
        await handler.HandleAsync(updatedEvent, EventUpdateHandler.UpdateAction.Create);
        backgroundTaskQueueFake.Queue.Should().HaveCount(1);
        await backgroundTaskQueueFake.Queue.First().Invoke(CancellationToken.None);

        // Assert
        pushUpClientFake.Command.Should().NotBeNull();
        pushUpClientFake.Command!.Title.Value.Should().Be("DinnerTimeChange");
        pushUpClientFake.Command.Body.Value.Should().Be("UserHasUpdated");
        pushUpClientFake.Command.Body.Args.Should().BeEquivalentTo(["Darki"]);
        pushUpClientFake.Command.UserIds.Should().BeEquivalentTo([2, 3]);
    }

    [Test]
    public async Task DoesNotCallPushUpClient_WhenEventDidNotEffectDinnerTime_Create()
    {
        // Arrange
        var pushUpClientFake = new PushUpContextFake();
        var backgroundTaskQueueFake = new BackgroundTaskQueueFake();
        
        var updatedEvent = EventInstanceTestData.CreateDefault(id: 1, userId: 1, date: dateTimeProviderFake.CurrentDate,
            dinnerTime: new TimeOnly(18, 00, 00));
        var effectiveEvent = EventInstanceTestData.CreateDefault(id: 2, userId: 1, date: dateTimeProviderFake.CurrentDate,
            dinnerTime: new TimeOnly(19, 00, 00));

        DbMock.Setup(c => c.EventInstances).ReturnsDbSet([updatedEvent, effectiveEvent]);
        DbMock.Setup(c => c.Users).ReturnsDbSet(userModels);
        var handler = new EventUpdateHandler(Db, pushUpClientFake, dateTimeProviderFake, backgroundTaskQueueFake, logger);

        // Act
        await handler.HandleAsync(updatedEvent, EventUpdateHandler.UpdateAction.Create);
        backgroundTaskQueueFake.Queue.Should().HaveCount(1);
        await (await backgroundTaskQueueFake.DequeueAsync(CancellationToken.None)).Invoke(CancellationToken.None);

        // Assert
        pushUpClientFake.Command.Should().BeNull();
    }
    
    [Test]
    public async Task CallsPusUpClient_WithExpectedCommand_Delete()
    {
        // Arrange
        var pushUpClientFake = new PushUpContextFake();
        var backgroundTaskQueueFake = new BackgroundTaskQueueFake();
        
        var deletedEvent = EventInstanceTestData.CreateDefault(userId: 1, date: dateTimeProviderFake.CurrentDate,
            dinnerTime: new TimeOnly(19, 00, 00));
        var effectiveEvent = EventInstanceTestData.CreateDefault(2, userId: 1, date: dateTimeProviderFake.CurrentDate,
            dinnerTime: new TimeOnly(18, 00, 00));

        DbMock.Setup(c => c.EventInstances).ReturnsDbSet([effectiveEvent]);
        DbMock.Setup(c => c.Users).ReturnsDbSet(userModels);
        
        var handler = new EventUpdateHandler(Db, pushUpClientFake, dateTimeProviderFake, backgroundTaskQueueFake, logger);

        // Act
        await handler.HandleAsync(deletedEvent, EventUpdateHandler.UpdateAction.Delete);
        
        backgroundTaskQueueFake.Queue.Should().HaveCount(1);
        await (await backgroundTaskQueueFake.DequeueAsync(CancellationToken.None)).Invoke(CancellationToken.None);

        // Assert
        pushUpClientFake.Command.Should().NotBeNull();
        pushUpClientFake.Command!.Title.Value.Should().Be("DinnerTimeChange");
        pushUpClientFake.Command.Body.Value.Should().Be("UserHasUpdated");
        pushUpClientFake.Command.Body.Args.Should().BeEquivalentTo(["Darki"]);
        pushUpClientFake.Command.UserIds.Should().BeEquivalentTo([2, 3]);
    }
}