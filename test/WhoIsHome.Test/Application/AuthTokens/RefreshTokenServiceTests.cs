using WhoIsHome.AuthTokens;

namespace WhoIsHome.Test.Application.AuthTokens;

[TestFixture]
public class RefreshTokenServiceTests : InMemoryDbTest
{
    private RefreshTokenService service = null!;

    [SetUp]
    public void SetUp()
    {
        service = new RefreshTokenService(DbFactory);
    }
    
    [TestFixture]
    private class CreateTokenAsync : RefreshTokenServiceTests
    {
        [Test]
        public async Task SaveNewTokenToDb()
        {
            // Act
            var result = await service.CreateTokenAsync(1, CancellationToken.None);
            
            // Assert
            result.Id.Should().Be(1);
            Db.RefreshTokens.Should().HaveCount(1);
            Db.RefreshTokens.Single().Id.Should().Be(1);
            Db.RefreshTokens.Single().UserId.Should().Be(1);
        }
    }
    
    [TestFixture]
    private class RefreshAsync : RefreshTokenServiceTests
    {
        [Test]
        public async Task SaveNewTokenToDb()
        {
            // Arrange
            var token = RefreshToken.Create(1);
            await SaveToDbAsync(token);
            
            // Act
            var result = await service.RefreshAsync(token.Token, CancellationToken.None);
            
            // Assert
            result.Id.Should().Be(2);
            Db.RefreshTokens.Should().HaveCount(2);
            Db.RefreshTokens.Single(t => t.Id == 1).ExpiredAt.Should().BeBefore(DateTime.Now);
        }
    }
    
    private async Task SaveToDbAsync(RefreshToken refreshToken)
    {
        var model = refreshToken.ToModel();
        await Db.RefreshTokens.AddAsync(model);
        await Db.SaveChangesAsync();
        Db.ChangeTracker.Clear();
    }
}