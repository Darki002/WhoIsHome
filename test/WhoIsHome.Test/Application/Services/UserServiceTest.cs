using Microsoft.AspNetCore.Identity;
using WhoIsHome.Entities;
using WhoIsHome.External.Models;
using WhoIsHome.Services;
using WhoIsHome.Test.Shared.Helper;
using WhoIsHome.Test.TestData;

namespace WhoIsHome.Test.Application.Services;

[TestFixture]
public class UserServiceTest : InMemoryDbTest
{
    private UserService service;

    [SetUp]
    public void SetUp()
    {
        service = new UserService(DbFactory, new PasswordHasher<User>());
    }

    [TestFixture]
    private class GetUserByEmailAsync : UserServiceTest
    {
        [Test]
        public async Task ReturnsExpectedUser_ByItsEmail()
        {
            // Arrange
            const string email = "expected.user@whoishome.dev";
            _ = await CreateAndSaveDefault(email: email);

            // Act
            var user = await service.GetUserByEmailAsync(email, CancellationToken.None);

            // Assert
            user.Should().NotBeNull();
            user!.Id.Should().Be(2);
            user.Email.Should().Be(email);
        }

        protected override async Task DbSetUpAsync()
        {
            var user = UserTestData.CreateDefaultUser();
            await Db.Users.AddAsync(user.ToModel());
            await Db.SaveChangesAsync();
        }
    }

    [TestFixture]
    private class CreateUserAsync : UserServiceTest
    {
        [Test]
        public async Task ReturnsEmailInUseResult_WhenEmailIsAlreadyInUse()
        {
            // Arrange
            await CreateAndSaveDefault();
            var user = UserTestData.CreateDefaultUser();

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

            // Act
            await service.CreateUserAsync(user.UserName, user.Email, user.Password, CancellationToken.None);

            // Assert
            Db.Users.Single().Password.Should().NotBeNullOrEmpty();
            Db.Users.Single().Password.Should().NotBe(password);
        }

        [Test]
        public async Task ReturnsNewCreatedUser()
        {
            // Arrange
            var user = UserTestData.CreateDefaultUser();

            // Act
            await service.CreateUserAsync(user.UserName, user.Email, user.Password, CancellationToken.None);

            // Asser
            var newUser = Db.Users.Single();
            newUser.Id.Should().Be(1);
            newUser.UserName.Should().Be(user.UserName);
            newUser.Email.Should().Be(user.Email);
        }
    }

    private async Task<UserModel> CreateAndSaveDefault(
        string userName = "Test User",
        string email = "test.user@whoishome.dev",
        string password = "test")
    {
        var user = UserTestData.CreateDefaultUser(userName, email, password);
        await Db.Users.AddAsync(user.ToModel());
        await Db.SaveChangesAsync();
        return user.ToModel();
    }
}