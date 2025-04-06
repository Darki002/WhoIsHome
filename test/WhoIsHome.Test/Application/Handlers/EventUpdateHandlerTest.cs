using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.EntityFrameworkCore;
using WhoIsHome.Aggregates.Mappers;
using WhoIsHome.External;
using WhoIsHome.External.Models;
using WhoIsHome.Handlers;
using WhoIsHome.Test.TestData;

namespace WhoIsHome.Test.Application.Handlers;

public class EventUpdateHandlerTest : InMemoryDbTest
{
    private readonly DateTimeProviderFake dateTimeProviderFake = new();

    private readonly ILogger<EventUpdateHandler> logger = NullLogger<EventUpdateHandler>.Instance;

    private readonly List<UserModel> userModels =
    [
        new() { Id = 1, UserName = "Darki" },
        new() { Id = 2, UserName = "Test" },
        new() { Id = 3, UserName = "Test2" }
    ];

    [Test]
    public async Task CallsPusUpClient_WithExpectedCommand_Create()
    {
        // Arrange
        var pushUpClientFake = new PushUpContextFake();
        var backgroundTaskQueueFake = new BackgroundTaskQueueFake();
        
        var updatedEvent = OneTimeEventTestData.CreateDefault(id: 1, userId: 1, date: dateTimeProviderFake.CurrentDate);

        var factory = GetDbFactoryMock(context =>
        {
            context.Setup(c => c.RepeatedEvents).ReturnsDbSet([]);
            context.Setup(c => c.OneTimeEvents).ReturnsDbSet([updatedEvent.ToModel()]);
            context.Setup(c => c.Users).ReturnsDbSet(userModels);
        });
        var handler = GetHandler(pushUpClientFake, backgroundTaskQueueFake, factory.Object);

        // Act
        await handler.HandleAsync(updatedEvent, EventUpdateHandler.UpdateAction.Create);
        backgroundTaskQueueFake.Queue.Should().HaveCount(1);
        await backgroundTaskQueueFake.Queue.First().Invoke(CancellationToken.None);

        // Assert
        pushUpClientFake.Command.Should().NotBeNull();
        pushUpClientFake.Command!.Title.Should().Be("Dinner time change");
        pushUpClientFake.Command.Body.Should().Be("Darki has an update for Today.");
        pushUpClientFake.Command.userIds.Should().BeEquivalentTo([2, 3]);
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

        var factory = GetDbFactoryMock(context =>
        {
            context.Setup(c => c.RepeatedEvents).ReturnsDbSet([]);
            context.Setup(c => c.OneTimeEvents).ReturnsDbSet([updatedEvent.ToModel(), effectiveEvent.ToModel()]);
            context.Setup(c => c.Users).ReturnsDbSet(userModels);
        });
        var handler = GetHandler(pushUpClientFake, backgroundTaskQueueFake, factory.Object);

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

        var factory = GetDbFactoryMock(context =>
        {
            context.Setup(c => c.RepeatedEvents).ReturnsDbSet([]);
            context.Setup(c => c.OneTimeEvents).ReturnsDbSet([effectiveEvent.ToModel()]);
            context.Setup(c => c.Users).ReturnsDbSet(userModels);
        });
        var handler = GetHandler(pushUpClientFake, backgroundTaskQueueFake, factory.Object);

        // Act
        await handler.HandleAsync(deletedEvent, EventUpdateHandler.UpdateAction.Delete);
        
        backgroundTaskQueueFake.Queue.Should().HaveCount(1);
        await (await backgroundTaskQueueFake.DequeueAsync(CancellationToken.None)).Invoke(CancellationToken.None);

        // Assert
        pushUpClientFake.Command.Should().NotBeNull();
        pushUpClientFake.Command!.Title.Should().Be("Dinner time change");
        pushUpClientFake.Command.Body.Should().Be("Darki has an update for Today.");
        pushUpClientFake.Command.userIds.Should().BeEquivalentTo([2, 3]);
    }

    private Mock<IDbContextFactory<WhoIsHomeContext>> GetDbFactoryMock(Action<Mock<WhoIsHomeContext>> setUp)
    {
        var context = new Mock<WhoIsHomeContext>(new DbContextOptions<WhoIsHomeContext>());
        setUp(context);
        var factory = new Mock<IDbContextFactory<WhoIsHomeContext>>();
        factory.Setup(f => f.CreateDbContextAsync(CancellationToken.None))
            .ReturnsAsync(context.Object);
        return factory;
    }

    private EventUpdateHandler GetHandler(
        PushUpContextFake pushUpContextFake,
        BackgroundTaskQueueFake backgroundTaskQueueFake,
        IDbContextFactory<WhoIsHomeContext> factory)
    {
        return new EventUpdateHandler(factory, pushUpContextFake, dateTimeProviderFake, backgroundTaskQueueFake, logger);
    }
}