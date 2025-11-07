using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using WhoIsHome.AuthTokens;
using WhoIsHome.Entities;
using WhoIsHome.Services;

namespace WhoIsHome.Test.Application.AuthTokens;

[TestFixture]
public class JwtTokenServiceTests
{
    private readonly DateTimeProviderFake dateTimeProviderFake = new();
    
    private IConfiguration configMock;
    private ILogger<JwtTokenService> loggerMock;
    private IUserService userService;
    
    [SetUp]
    public void SetUp()
    {
        var mockUserService = new Mock<IUserService>();

        mockUserService.Setup(u => u.GetAsync(1, CancellationToken.None))
            .ReturnsAsync(() => new User("Test", "test@whoishome.dev", "test") { Id = 1 });
        
        var mockConfiguration = new Mock<IConfiguration>();
        var sectionMock = new Mock<IConfigurationSection>();
        
        sectionMock.Setup(section => section["Issuer"]).Returns("TestIssuer");
        sectionMock.Setup(section => section["Audience"]).Returns("TestAudience");
        sectionMock.Setup(section => section["ExpiresInMinutes"]).Returns("60");
        
        mockConfiguration.Setup(config => config.GetSection("JwtSettings")).Returns(sectionMock.Object);
        mockConfiguration.Setup(config => config["JWT_SECRET_KEY"])
            .Returns("hakjlshdfkljashdfkljahsdfkjhalksdhfkljashdfkljhaskljdfhakjlsdhfkjh");
        
        configMock = mockConfiguration.Object;
        userService = mockUserService.Object;
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
            var refreshToken = RefreshToken.Generate(1, dateTimeProviderFake.Now);
            refreshService
                .Setup(s => s.CreateTokenAsync(1, CancellationToken.None))
                .ReturnsAsync(refreshToken);

            var service = new JwtTokenService(configMock, refreshService.Object, userService, loggerMock);
            var user = new User( "", "", "") { Id = 1 };
            
            // Act
            var result = await service.GenerateTokenAsync(user, CancellationToken.None);
            
            // Assert
            result.HasError.Should().BeFalse();
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
            
            var token = RefreshToken.Generate(1, dateTimeProviderFake.Now);
            
            var refreshService = new Mock<IRefreshTokenService>();
            refreshService
                .Setup(s => s.RefreshAsync(oldToken, CancellationToken.None))
                .ReturnsAsync(() => new ValidRefreshTokenResult(token, null));

            var service = new JwtTokenService(configMock, refreshService.Object, userService, loggerMock);
            
            // Act
            var result = await service.RefreshTokenAsync(oldToken, CancellationToken.None);
            
            // Assert
            result.JwtToken.Should().NotBeEmpty();
            result.RefreshToken.Should().NotBeEmpty();
            refreshService.Verify(s => s.RefreshAsync(oldToken,  CancellationToken.None));
        }
    }
}