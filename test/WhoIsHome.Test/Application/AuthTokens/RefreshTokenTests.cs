using WhoIsHome.AuthTokens;

namespace WhoIsHome.Test.Application.AuthTokens;

[TestFixture]
public class RefreshTokenTests
{
    private readonly DateTimeProviderFake dateTimeProviderFake = new();
    
    [TestFixture]
    private class Create : RefreshTokenTests
    {
        [Test]
        public void ReturnsNewToken()
        {
            // Act
            var token = RefreshToken.Create(1, dateTimeProviderFake.Now);
            
            // Assert
            token.Id.Should().BeNull();
            token.UserId.Should().Be(1);
            token.Token.Should().NotBeEmpty();
            token.Issued.Should().Be(dateTimeProviderFake.Now);
            token.ExpiredAt.Should().Be(dateTimeProviderFake.Now.AddDays(14));
        }
    }
}