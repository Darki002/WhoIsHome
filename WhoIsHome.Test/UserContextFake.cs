using WhoIsHome.Aggregates;
using WhoIsHome.Shared.Authentication;

namespace WhoIsHome.Test;

public class UserContextFake : IUserContext
{
    private AuthenticatedUser authenticatedUser = null!;
    
    public int UserId { get; } = 0;
    
    public Task<AuthenticatedUser> GetCurrentUserAsync()
    {
        return Task.FromResult(authenticatedUser);
    }

    public Task<AuthenticatedUser> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(authenticatedUser);
    }

    public bool IsUserPermitted(int permittedUserId)
    {
        return authenticatedUser.Id == permittedUserId;
    }

    public void SetUser(User user)
    {
        authenticatedUser = new AuthenticatedUser
        {
            Id = user.Id!.Value,
            UserName = user.UserName,
            Email = user.Email,
            PasswordHash = user.Password
        };
    }
}