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
            var token = RefreshToken.Create(1, dateTimeProviderFake);
            
            // Assert
            token.Id.Should().BeNull();
            token.UserId.Should().Be(1);
            token.Token.Should().NotBeEmpty();
            token.Issued.Should().Be(dateTimeProviderFake.Now);
            token.ExpiredAt.Should().Be(dateTimeProviderFake.Now.AddDays(14));
        }
    }
    
    [TestFixture]
    private class Refresh : RefreshTokenTests
    {
        [Test]
        public void InvalidatesOldToken_ReturnsNewToken()
        {
            // Arrange
            var token = RefreshToken.Create(1, dateTimeProviderFake);
            
            // Act
            var newToken = token.Refresh();
            
            // Assert
            token.ExpiredAt.Should().Be(dateTimeProviderFake.Now);
            
            newToken.Id.Should().BeNull();
            newToken.UserId.Should().Be(1);
            newToken.Token.Should().NotBeEmpty();
            newToken.Issued.Should().Be(dateTimeProviderFake.Now);
            newToken.ExpiredAt.Should().Be(dateTimeProviderFake.Now.AddDays(14));
        }
    }
    
    [TestFixture]
    private class IsValid : RefreshTokenTests
    {
        [Test]
        public void RetunrsTrue_WhenTokenIsValid()
        {
            // Arrange
            var token = RefreshToken.Create(1, dateTimeProviderFake);
            
            // Act
            var isValid = token.IsValid();
            
            // Assert
            isValid.Should().BeTrue();
        }
        
        [Test]
        public void RetunrsFalse_WhenExpiredIsSet()
        {
            // Arrange
            var issued = new DateTime(2024, 10, 21);
            var expiresAt = dateTimeProviderFake.Now.AddHours(1);
            var token = new RefreshToken(1, 1, "", issued, expiresAt, dateTimeProviderFake);
            token.Refresh();
            
            // Act
            var isValid = token.IsValid();
            
            // Assert
            isValid.Should().BeFalse();
        }
    }
}