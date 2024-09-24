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
        return await GetCurrentUserAsync(default);
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
        
        CheckEmailAddress(user.Email);
        
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

    private void CheckEmailAddress(string expectedEmail)
    {
        var email = httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Email)?.Value;
        if (expectedEmail != email)
        {
            throw new UnauthorizedAccessException("Email Address in the Request must match the Email of the User.");
        }
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