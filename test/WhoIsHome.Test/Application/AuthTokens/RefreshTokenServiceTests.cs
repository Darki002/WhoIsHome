using Microsoft.Extensions.Logging;
using Moq;
using WhoIsHome.AuthTokens;
using WhoIsHome.External.Models;

namespace WhoIsHome.Test.Application.AuthTokens;

[TestFixture]
public class RefreshTokenServiceTests : InMemoryDbTest
{
    private readonly DateTimeProviderFake dateTimeProviderFake = new();
    
    private RefreshTokenService service = null!;

    [SetUp]
    public void SetUp()
    {
        var logger = Mock.Of<ILogger<RefreshTokenService>>();
        service = new RefreshTokenService(DbFactory, dateTimeProviderFake, logger);
    }
    
    [TestFixture]
    private class CreateTokenAsync : RefreshTokenServiceTests
    {
        [Test]
        public async Task SaveNewTokenToDb()
        {
            // Act
            var result = await service.CreateTokenAsync(1, CancellationToken.None);
            
            // Assert
            result.Id.Should().Be(1);
            Db.RefreshTokens.Should().HaveCount(1);
            Db.RefreshTokens.Single().Id.Should().Be(1);
            Db.RefreshTokens.Single().UserId.Should().Be(1);
        }
    }
    
    [TestFixture]
    private class RefreshAsync : RefreshTokenServiceTests
    {
        [Test]
        public async Task SavesNewTokenToDb()
        {
            // Arrange
            var token = RefreshToken.Create(1, dateTimeProviderFake.Now);
            await SaveToDbAsync(token);
            
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
            var token = new RefreshToken(1, 1, "", issued, expiresAt);
            await SaveToDbAsync(token);
            
            // Act
            var result = await service.RefreshAsync(token.Token, CancellationToken.None);
            
            // Assert
            result.HasError.Should().BeTrue();
        }
    }
    
    private async Task SaveToDbAsync(RefreshToken refreshToken)
    {
        var model = new RefreshTokenModel
        {
            Id = 0,
            Token = refreshToken.Token,
            ExpiredAt = refreshToken.ExpiredAt,
            Issued = refreshToken.Issued,
            UserId = refreshToken.UserId
        };
        
        await Db.RefreshTokens.AddAsync(model);
        await Db.SaveChangesAsync();
        Db.ChangeTracker.Clear();
    }
}