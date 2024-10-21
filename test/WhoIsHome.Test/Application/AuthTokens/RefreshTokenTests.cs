using WhoIsHome.AuthTokens;

namespace WhoIsHome.Test.Application.AuthTokens;

[TestFixture]
public class RefreshTokenTests
{
    [TestFixture]
    private class Create
    {
        [Test]
        public void ReturnsNewToken()
        {
            // Act
            var token = RefreshToken.Create(1);
            
            // Assert
            token.Id.Should().BeNull();
            token.UserId.Should().Be(1);
            token.Token.Should().NotBeEmpty();
            token.Issued.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
            token.ExpiredAt.Should().BeCloseTo(DateTime.Now.AddDays(14), TimeSpan.FromSeconds(5));
        }
    }
    
    [TestFixture]
    private class Refresh
    {
        [Test]
        public void InvalidatesOldToken_ReturnsNewToken()
        {
            // Arrange
            var token = RefreshToken.Create(1);
            
            // Act
            var newToken = token.Refresh();
            
            // Assert
            token.ExpiredAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
            
            newToken.Id.Should().BeNull();
            newToken.UserId.Should().Be(1);
            newToken.Token.Should().NotBeEmpty();
            newToken.Issued.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
            newToken.ExpiredAt.Should().BeCloseTo(DateTime.Now.AddDays(14), TimeSpan.FromSeconds(5));
        }
    }
    
    [TestFixture]
    private class IsValid
    {
        [Test]
        public void RetunrsTrue_WhenTokenIsValid()
        {
            // Arrange
            var token = RefreshToken.Create(1);
            
            // Act
            var isValid = token.IsValid(1);
            
            // Assert
            isValid.Should().BeTrue();
        }
        
        [Test]
        public void RetunrsFalse_WhenUserIdDoesNotMatch()
        {
            // Arrange
            var token = RefreshToken.Create(1);
            
            // Act
            var isValid = token.IsValid(2);
            
            // Assert
            isValid.Should().BeFalse();
        }
        
        [Test]
        public void RetunrsFalse_WhenExpiredIsSet()
        {
            // Arrange
            var issued = new DateTime(2024, 10, 21);
            var expiresAt = DateTime.Now.AddHours(1);
            var token = new RefreshToken(1, 1, "", issued, expiresAt);
            token.Refresh();
            
            // Act
            var isValid = token.IsValid(1);
            
            // Assert
            isValid.Should().BeFalse();
        }
    }
}