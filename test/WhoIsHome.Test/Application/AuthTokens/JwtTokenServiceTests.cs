using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using WhoIsHome.Aggregates;
using WhoIsHome.AuthTokens;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.Test.Application.AuthTokens;

[TestFixture]
public class JwtTokenServiceTests
{
    private IConfiguration configMock;
    private ILogger<JwtTokenService> loggerMock;
    
    [SetUp]
    public void SetUp()
    {
        var mockConfiguration = new Mock<IConfiguration>();
        var sectionMock = new Mock<IConfigurationSection>();
        
        sectionMock.Setup(section => section["Issuer"]).Returns("TestIssuer");
        sectionMock.Setup(section => section["Audience"]).Returns("TestAudience");
        sectionMock.Setup(section => section["ExpiresInMinutes"]).Returns("60");
        
        mockConfiguration.Setup(config => config.GetSection("JwtSettings")).Returns(sectionMock.Object);
        mockConfiguration.Setup(config => config["JWT_SECRET_KEY"])
            .Returns("hakjlshdfkljashdfkljahsdfkjhalksdhfkljashdfkljhaskljdfhakjlsdhfkjh");
        
        configMock = mockConfiguration.Object;
        loggerMock = Mock.Of<ILogger<JwtTokenService>>();
    }
    
    [TestFixture]
    private class GenerateTokenAsync : JwtTokenServiceTests
    {
        [Test]
        public async Task ReturnsNewTokens()
        {
            // Arrange
            var refreshService = new Mock<IRefreshTokenService>();
            refreshService
                .Setup(s => s.CreateTokenAsync(1, CancellationToken.None))
                .ReturnsAsync(() => RefreshToken.Create(1));

            var service = CreateService(refreshService.Object);
            var user = new User(1, "", "", "");
            
            // Act
            var result = await service.GenerateTokenAsync(user, CancellationToken.None);
            
            // Assert
            result.JwtToken.Should().NotBeEmpty();
            result.RefreshToken.Should().NotBeEmpty();
            refreshService.Verify(s => s.CreateTokenAsync(1, CancellationToken.None));
        }
    }
    
    [TestFixture]
    private class RefreshTokenAsync : JwtTokenServiceTests
    {
        [Test]
        public async Task ReturnsNewToken()
        {
            // Arrange
            const string oldToken = "old-token";
            var refreshService = new Mock<IRefreshTokenService>();
            refreshService
                .Setup(s => s.RefreshAsync(oldToken, 1, CancellationToken.None))
                .ReturnsAsync(() => RefreshToken.Create(1));

            var service = CreateService(refreshService.Object);
            var user = new User(1, "", "", "");
            
            // Act
            var result = await service.RefreshTokenAsync(user, oldToken, CancellationToken.None);
            
            // Assert
            result.JwtToken.Should().NotBeEmpty();
            result.RefreshToken.Should().NotBeEmpty();
            refreshService.Verify(s => s.RefreshAsync(oldToken, 1, CancellationToken.None));
        }
    }

    private JwtTokenService CreateService(IRefreshTokenService refreshTokenService)
    {
        return new JwtTokenService(configMock, refreshTokenService, loggerMock);
    }
}