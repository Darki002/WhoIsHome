using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.EntityFrameworkCore;
using WhoIsHome.AuthTokens;

namespace WhoIsHome.Test.Application.AuthTokens;

[TestFixture]
public class RefreshTokenServiceTests : DbMockTest
{
    private readonly DateTimeProviderFake dateTimeProviderFake = new();
    
    private RefreshTokenService service = null!;

    [SetUp]
    public void SetUp()
    {
        var logger = Mock.Of<ILogger<RefreshTokenService>>();
        service = new RefreshTokenService(Db, dateTimeProviderFake, logger);
    }
    
    [TestFixture]
    private class CreateTokenAsync : RefreshTokenServiceTests
    {
        [Test]
        public async Task SaveNewTokenToDb()
        {
            // Arrange
            DbMock.Setup(c => c.RefreshTokens).ReturnsDbSet([]);

            DbMock.AddChangeTrackingWithCt(
                c => c.RefreshTokens.AddAsync(
                    It.IsAny<RefreshToken>(),
                    It.IsAny<CancellationToken>()),
                e => { e.Id = 1; });
            
            // Act
            var result = await service.CreateTokenAsync(1, CancellationToken.None);
            
            // Assert
            result.Id.Should().Be(1);
            result.UserId.Should().Be(1);
        }
    }
    
    [TestFixture]
    private class RefreshAsync : RefreshTokenServiceTests
    {
        [Test]
        public async Task SavesNewTokenToDb()
        {
            // Arrange
            var token = RefreshToken.Generate(1, dateTimeProviderFake.Now);
            DbMock.Setup(c => c.RefreshTokens).ReturnsDbSet([token]);
            
            // Act
            var result = await service.RefreshAsync(token.Token, CancellationToken.None);
            
            // Assert
            result.HasError.Should().BeFalse();
            result.Value.Id.Should().Be(2);
            Db.RefreshTokens.Should().HaveCount(2);
            Db.RefreshTokens.Single(t => t.Id == 1).ExpiredAt.Should().BeBefore(DateTime.Now);
        }
        
        [Test]
        public async Task ReturnsError_WhenExpired()
        {
            // Arrange
            
            var issued = new DateTime(2024, 10, 21);
            var expiresAt = dateTimeProviderFake.Now.AddHours(-1);
            var token = new RefreshToken(1, "", issued, expiresAt);
            DbMock.Setup(c => c.RefreshTokens).ReturnsDbSet([token]);
            
            // Act
            var result = await service.RefreshAsync(token.Token, CancellationToken.None);
            
            // Assert
            result.HasError.Should().BeTrue();
        }
    }
}