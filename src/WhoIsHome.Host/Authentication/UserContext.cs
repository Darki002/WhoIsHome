using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using WhoIsHome.DataAccess;
using WhoIsHome.DataAccess.Models;
using WhoIsHome.Shared.Authentication;
using WhoIsHome.Shared.Exceptions;

namespace WhoIsHome.Host.Authentication;

public class UserContext(
    IHttpContextAccessor httpContextAccessor,
    IDbContextFactory<WhoIsHomeContext> contextFactory,
    ILogger<UserContext> logger) : IUserContext
{
    private AuthenticatedUser? authenticatedUserCache = null;

    private int? idCache = null;

    public int UserId => GetId() ?? throw new InvalidClaimsException("No User is present in the Request.");

    public async Task<AuthenticatedUser> GetCurrentUserAsync()
    {
        return await GetCurrentUserAsync(default);
    }

    public async Task<AuthenticatedUser> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        if (GetId() is null)
        {
            logger.LogInformation("(Invalid Claims) | No User ID found in request");
            throw new InvalidClaimsException("No User is present in the Request.");
        }

        if (authenticatedUserCache is not null)
        {
            return authenticatedUserCache;
        }

        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var user = await context.Users.AsNoTracking().SingleAsync(u => u.Id == UserId, cancellationToken);

        CheckEmailAddress(user);

        authenticatedUserCache = user.MapFromDb();
        return authenticatedUserCache;
    }

    public bool IsUserPermitted(int permittedUserId)
    {
        return GetId() == permittedUserId;
    }

    private int? GetId() => idCache ?? GetIdFromClaims();

    private int? GetIdFromClaims()
    {
        var idString = httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        idCache = idString is not null ? int.Parse(idString) : null;
        return idCache;
    }

    private void CheckEmailAddress(UserModel user)
    {
        var email = httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Email)?.Value;
        if (user.Email != email)
        {
            logger.LogInformation("(Invalid Claims) | For User {UserId} with unexpected Email {Email}", user.Id,
                email);
            throw new InvalidClaimsException("Email Address in the Request must match the Email of the User.");
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