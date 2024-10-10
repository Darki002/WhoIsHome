using Microsoft.AspNetCore.Identity;
using WhoIsHome.Aggregates;
using WhoIsHome.DataAccess.Models;
using WhoIsHome.Services;
using WhoIsHome.Shared.Exceptions;

namespace WhoIsHome.Test.Application.Services;

[TestFixture]
public class UserAggregateServiceTest : InMemoryDbTest
{
    private UserAggregateService service;

    [SetUp]
    public void SetUp()
    {
        service = new UserAggregateService(Db, new PasswordHasher<User>());
    }
    
    [TestFixture]
    private class GetUserByEmailAsync : UserAggregateServiceTest
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
            var user = CreateDefaultUser();
            await Db.Users.AddAsync(user);
            await Db.SaveChangesAsync();
        }
    }

    [TestFixture]
    private class CreateUserAsync : UserAggregateServiceTest
    {
        [Test]
        public async Task ThrowsEmailInUseException_WhenEmailIsAlreadyInUse()
        {
            // Arrange
            await CreateAndSaveDefault();
            var user = CreateDefaultUser();
            
            // Act
            var act = async () => await service.CreateUserAsync(user.UserName, user.Email, user.Password, CancellationToken.None);
            
            // Assert
            await act.Should().ThrowAsync<EmailInUseException>();
        }

        [Test]
        public async Task HashesPassword_BeforeInserting()
        {
            // Arrange
            const string password = "test";
            var user = CreateDefaultUser(password: password);
            
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
            var user = CreateDefaultUser();
            
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
        var user = CreateDefaultUser(userName, email, password);
        await Db.Users.AddAsync(user);
        await Db.SaveChangesAsync();
        return user;
    }
    
    private static UserModel CreateDefaultUser(
        string userName = "Test User", 
        string email = "test.user@whoishome.dev", 
        string password = "test")
    {
        return new UserModel
        {
            UserName = userName,
            Email = email,
            Password = password
        };
    }
}