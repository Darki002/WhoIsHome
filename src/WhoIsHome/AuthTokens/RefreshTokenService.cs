using WhoIsHome.DataAccess;
using WhoIsHome.Shared.Authentication;

namespace WhoIsHome.AuthTokens;

public class RefreshTokenService(WhoIsHomeContext context, IUserContext userContext)
{
    public RefreshToken GenerateToken()
    {
        var tokenString = CreateToken();
        var token = RefreshToken.Create(userContext.UserId, tokenString);
        return token;
    }

    private static string CreateToken()
    {
        throw new NotImplementedException();
    }
}