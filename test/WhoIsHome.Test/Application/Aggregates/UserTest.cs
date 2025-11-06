using WhoIsHome.Entities;

namespace WhoIsHome.Test.Application.Aggregates;

[TestFixture]
public class UserTest
{
    [TestFixture]
    private class Create : UserTest
    {
        [Test]
        public void ReturnsNewUser_RepresentingAUserFromTheGivenData()
        {
            // Arrange
            const string userName = "Darki123";
            const string email = "darki@whoishome.dev";
            const string password = "securePassword1234";
        
            // Act
            var result = new User(userName, email, password);
        
            // Assert
            result.Id.Should().BeNull();
            result.UserName.Should().Be(userName);
            result.Email.Should().Be(email);
            result.Password.Should().Be(password);
        }

        [Test]
        public void ReturnsValidationError_WhenEmailIsInvalidFormat()
        {
            // Arrange
            const string userName = "Darki123";
            const string invalidEmail = "missingAt.dev";
            const string password = "securePassword1234";
            var user = new User(userName, invalidEmail, password);
        
            // Act
            var result = user.Validate();
        
            // Assert
            result.Should().HaveCount(1);
        }
    
        [Test]
        public void ReturnsValidationError_WhenUserNameIsTooLong()
        {
            // Arrange
            var userName = string.Join("", Enumerable.Repeat('a', 32));
            const string invalidEmail = "darki@whoishome.dev";
            const string password = "securePassword1234";
            var user = new User(userName, invalidEmail, password);
        
            // Act
            var result = user.Validate();
        
            // Assert
            result.Should().HaveCount(1);
        }
        
        [Test]
        public void ThrowsInvalidModelException_WhenUserNameIsTooShort()
        {
            // Arrange
            const string userName = "";
            const string invalidEmail = "darki@whoishome.dev";
            const string password = "securePassword1234";
            var user = new User(userName, invalidEmail, password);
        
            // Act
            var result = user.Validate();
        
            // Assert
            result.Should().HaveCount(1);
        }
    }
}