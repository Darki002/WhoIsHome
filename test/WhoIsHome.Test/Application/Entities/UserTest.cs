using WhoIsHome.Entities;

namespace WhoIsHome.Test.Application.Entities;

[TestFixture]
public class UserTest
{
    [TestFixture]
    private class Validate : UserTest
    {
        [Test]
        public void ReturnsNoValidationErrors_WhenUserDataIsCorrect()
        {
            // Arrange
            const string userName = "Darki123";
            const string email = "darki@whoishome.dev";
            const string password = "securePassword1234";
            var user = new User
            {
                UserName = userName,
                Email = email,
                Password = password
            };
        
            // Act
            var result = user.Validate();
        
            // Assert
            result.Should().HaveCount(0);
        }

        [Test]
        public void ReturnsValidationError_WhenEmailIsInvalidFormat()
        {
            // Arrange
            const string userName = "Darki123";
            const string invalidEmail = "missingAt.dev";
            const string password = "securePassword1234";
            var user = new User
            {
                UserName = userName,
                Email = invalidEmail,
                Password = password
            };
        
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
            var user = new User
            {
                UserName = userName,
                Email = invalidEmail,
                Password = password
            };
        
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
            var user = new User
            {
                UserName = userName,
                Email = invalidEmail,
                Password = password
            };
        
            // Act
            var result = user.Validate();
        
            // Assert
            result.Should().HaveCount(1);
        }
    }
}