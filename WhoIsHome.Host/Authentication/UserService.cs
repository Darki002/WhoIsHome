using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using WhoIsHome.DataAccess;
using WhoIsHome.DataAccess.Models;
using WhoIsHome.Shared.Authentication;

namespace WhoIsHome.Host.Authentication;

public class UserService(IHttpContextAccessor httpContextAccessor, WhoIsHomeContext context) : IUserService
{
    private AuthenticatedUser? authenticatedUserCache = null;
    
    private int? idCache = null;
    
    private int? UserId => idCache ?? GetIdFromClaims();

    public async Task<AuthenticatedUser> GetCurrentUserAsync()
    {
        if (UserId is null)
        {
            throw new InvalidOperationException("No User is present in the Request.");
        }

        if (authenticatedUserCache is not null)
        {
            return authenticatedUserCache;
        }

        var user = await context.Users.SingleAsync(u => u.Id == UserId);
        authenticatedUserCache = user.MapFromDb();
        return authenticatedUserCache;
    }

    public async Task<AuthenticatedUser> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        if (UserId is null)
        {
            throw new InvalidOperationException("No User is present in the Request.");
        }

        if (authenticatedUserCache is not null)
        {
            return authenticatedUserCache;
        }

        var user = await context.Users.SingleAsync(u => u.Id == UserId, cancellationToken);
        authenticatedUserCache = user.MapFromDb();
        return authenticatedUserCache;
    }

    public bool IsUserPermitted(int permittedUserId)
    {
        return UserId == permittedUserId;
    }

    private int? GetIdFromClaims()
    {
        var idString = httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        idCache = idString is not null ? int.Parse(idString) : null;
        return idCache;
    }
}

public static class Mapper
{
    public static AuthenticatedUser MapFromDb(this UserModel dbModel)
    {
        return new AuthenticatedUser
        {
            Id = dbModel.Id,
            UserName = dbModel.UserName,
            Email = dbModel.Email,
            PasswordHash = dbModel.Password
        };
    }
}