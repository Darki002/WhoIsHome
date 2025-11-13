using Microsoft.AspNetCore.Identity;
using Moq;
using Moq.EntityFrameworkCore;
using WhoIsHome.Entities;
using WhoIsHome.Services;
using WhoIsHome.Test.TestData;

namespace WhoIsHome.Test.Application.Services;

[TestFixture]
public class UserServiceTest : DbMockTest
{
    private UserService service;

    [SetUp]
    public void SetUp()
    {
        service = new UserService(Db, new PasswordHasher<User>());
    }

    [TestFixture]
    private class GetAsync : UserServiceTest
    {
        [Test]
        public async Task ReturnsExpectedUser_ByItsId()
        {
            // Arrange
            const string email = "expected.user@whoishome.dev";
            
            var user1 = UserTestData.CreateDefaultUser();
            var user2 = UserTestData.CreateDefaultUser(id: 2, userName: "Test", email: email, password: "Test");
            DbMock.Setup(c => c.Users).ReturnsDbSet([user1, user2]);

            // Act
            var user = await service.GetAsync(2, CancellationToken.None);

            // Assert
            user.Should().NotBeNull();
            user!.Id.Should().Be(2);
            user.Email.Should().Be(email);
        }
    }
    
    [TestFixture]
    private class GetUserByEmailAsync : UserServiceTest
    {
        [Test]
        public async Task ReturnsExpectedUser_ByItsEmail()
        {
            // Arrange
            const string email = "expected.user@whoishome.dev";
            
            var user1 = UserTestData.CreateDefaultUser();
            var user2 = UserTestData.CreateDefaultUser(id: 2, userName: "Test", email: email, password: "Test");
            DbMock.Setup(c => c.Users).ReturnsDbSet([user1, user2]);

            // Act
            var user = await service.GetUserByEmailAsync(email, CancellationToken.None);

            // Assert
            user.Should().NotBeNull();
            user!.Id.Should().Be(2);
            user.Email.Should().Be(email);
        }
    }

    [TestFixture]
    private class CreateUserAsync : UserServiceTest
    {
        [Test]
        public async Task ReturnsEmailInUseResult_WhenEmailIsAlreadyInUse()
        {
            // Arrange
            const string email = "test@test.dev";
            var existingUser = UserTestData.CreateDefaultUser(email: email);
            DbMock.Setup(c => c.Users).ReturnsDbSet([existingUser]);
            
            var user = UserTestData.CreateDefaultUser(email: email);

            // Act
            var result =
                await service.CreateUserAsync(user.UserName, user.Email, user.Password, CancellationToken.None);

            // Assert
            result.HasErrors.Should().BeTrue();
            result.ValidationErrors.Should().HaveCount(1);
        }

        [Test]
        public async Task ReturnsUsernameTooLongResult_WhenUsernameIsTooLong()
        {
            // Arrange
            var userName = string.Join("", Enumerable.Repeat('a', 40));
            var user = UserTestData.CreateDefaultUser(userName: userName);
            DbMock.Setup(c => c.Users).ReturnsDbSet([]);

            // Act
            var result = await service.CreateUserAsync(
                user.UserName, 
                user.Email, 
                user.Password, 
                CancellationToken.None);

            // Assert
            result.HasErrors.Should().BeTrue();
            result.ValidationErrors.Should().HaveCount(1);
        }

        [Test]
        public async Task HashesPassword_BeforeInserting()
        {
            // Arrange
            const string password = "test";
            var user = UserTestData.CreateDefaultUser(password: password);
            DbMock.Setup(c => c.Users).ReturnsDbSet([]);
            DbMock.AddChangeTracking(c => c.Users.Add(It.IsAny<User>()));

            // Act
            var result = await service.CreateUserAsync(user.UserName, user.Email, user.Password, CancellationToken.None);

            // Assert
            var errors = string.Join(", ", result.ValidationErrors.Select(e => e.Message));
            result.HasErrors.Should().BeFalse("Not expected Validation Errors: {0}", errors);
            result.Value.Should().NotBeNull();
            result.Result.Password.Should().NotBeNullOrEmpty();
            result.Result.Password.Should().NotBe(password);
        }

        [Test]
        public async Task ReturnsNewCreatedUser()
        {
            // Arrange
            var user = UserTestData.CreateDefaultUser();
            DbMock.Setup(c => c.Users).ReturnsDbSet([]);
            DbMock.AddChangeTracking(c => c.Users.Add(It.IsAny<User>()), e => { e.Id = 42; });

            // Act
            var result = await service.CreateUserAsync(user.UserName, user.Email, user.Password, CancellationToken.None);

            // Asser
            result.HasErrors.Should().BeFalse();
            result.Value.Should().NotBeNull();
            result.Result.Id.Should().Be(42);
            result.Result.UserName.Should().Be(user.UserName);
            result.Result.Email.Should().Be(user.Email);
        }
    }
}