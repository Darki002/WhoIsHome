using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.EntityFrameworkCore;
using WhoIsHome.Entities;
using WhoIsHome.External;
using WhoIsHome.External.Database;
using WhoIsHome.Handlers;
using WhoIsHome.Test.TestData;

namespace WhoIsHome.Test.Application.Handlers;

public class EventUpdateHandlerTest : InMemoryDbTest
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
        
        var updatedEvent = OneTimeEventTestData.CreateDefault(id: 1, userId: 1, date: dateTimeProviderFake.CurrentDate);

        var dbMock = GetDbFactoryMock(context =>
        {
            context.Setup(c => c.EventInstances).ReturnsDbSet([updatedEvent]);
            context.Setup(c => c.Users).ReturnsDbSet(userModels);
        });
        var handler = new EventUpdateHandler(dbMock.Object, pushUpClientFake, dateTimeProviderFake, backgroundTaskQueueFake, logger);

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
        
        var updatedEvent = OneTimeEventTestData.CreateDefault(id: 1, userId: 1, date: dateTimeProviderFake.CurrentDate,
            dinnerTime: new TimeOnly(18, 00, 00));
        var effectiveEvent = OneTimeEventTestData.CreateDefault(id: 2, userId: 1, date: dateTimeProviderFake.CurrentDate,
            dinnerTime: new TimeOnly(19, 00, 00));

        var dbMock = GetDbFactoryMock(context =>
        {
            context.Setup(c => c.EventInstances).ReturnsDbSet([updatedEvent.ToModel(), effectiveEvent.ToModel()]);
            context.Setup(c => c.Users).ReturnsDbSet(userModels);
        });
        var handler = new EventUpdateHandler(dbMock.Object, pushUpClientFake, dateTimeProviderFake, backgroundTaskQueueFake, logger);

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
        
        var deletedEvent = OneTimeEventTestData.CreateDefault(1, userId: 1, date: dateTimeProviderFake.CurrentDate,
            dinnerTime: new TimeOnly(19, 00, 00));
        var effectiveEvent = OneTimeEventTestData.CreateDefault(2, userId: 1, date: dateTimeProviderFake.CurrentDate,
            dinnerTime: new TimeOnly(18, 00, 00));

        var dbMock = GetDbFactoryMock(context =>
        {
            context.Setup(c => c.EventInstances).ReturnsDbSet([effectiveEvent.ToModel()]);
            context.Setup(c => c.Users).ReturnsDbSet(userModels);
        });
        
        var handler = new EventUpdateHandler(dbMock.Object, pushUpClientFake, dateTimeProviderFake, backgroundTaskQueueFake, logger);

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

    private static Mock<WhoIsHomeContext> GetDbFactoryMock(Action<Mock<WhoIsHomeContext>> setUp)
    {
        var context = new Mock<WhoIsHomeContext>(new DbContextOptions<WhoIsHomeContext>());
        setUp(context);
        return context;
    }
}