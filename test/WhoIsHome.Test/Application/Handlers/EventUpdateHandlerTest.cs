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

    private readonly PushUpClientFake pushUpClientFake = new();

    private readonly ILogger<EventUpdateHandler> logger = NullLogger<EventUpdateHandler>.Instance;

    private readonly List<UserModel> userModels =
    [
        new() { Id = 1, UserName = "Darki" },
        new() { Id = 2, UserName = "Test" },
        new() { Id = 3, UserName = "Test2" }
    ];

    [Test]
    public async Task CallsPusUpClient_WithExpectedCommand()
    {
        // Arrange
        var updatedEvent = OneTimeEventTestData.CreateDefault(id: 1, userId: 1);
        
        var factory = GetDbFactoryMock(context =>
        {
            context.Setup(c => c.RepeatedEvents).ReturnsDbSet([]);
            context.Setup(c => c.OneTimeEvents).ReturnsDbSet([updatedEvent.ToModel()]);
            context.Setup(c => c.Users).ReturnsDbSet(userModels);
        });
        var handler = GetHandler(factory.Object);
        
        // Act
        await handler.HandleAsync(updatedEvent, CancellationToken.None);
        
        // Assert
        pushUpClientFake.Command.Should().NotBeNull();
        pushUpClientFake.Command!.Title.Should().Be("Event Update");
        pushUpClientFake.Command!.Body.Should().Be("Darki has entered a new Event for Today.");
        pushUpClientFake.Command.userIds.Should().BeEquivalentTo([2, 3]);
    }

    private Mock<IDbContextFactory<WhoIsHomeContext>> GetDbFactoryMock(Action<Mock<WhoIsHomeContext>> setUp)
    {
        var context = new Mock<WhoIsHomeContext>();
        setUp(context);
        var factory = new Mock<IDbContextFactory<WhoIsHomeContext>>();
        factory.Setup(f => f.CreateDbContextAsync(CancellationToken.None))
            .ReturnsAsync(context.Object);
        return factory;
    }

    private EventUpdateHandler GetHandler(IDbContextFactory<WhoIsHomeContext> factory) =>
        new EventUpdateHandler(factory, dateTimeProviderFake, pushUpClientFake, logger);
}