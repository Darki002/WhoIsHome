using WhoIsHome.Entities;
using WhoIsHome.Shared.Authentication;

namespace WhoIsHome.Test;

public class UserContextProviderFake : IUserContextProvider
{
    private AuthenticatedUser authenticatedUser = null!;

    public int UserId => authenticatedUser.Id;
    
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

    public void SetUser(User user, int id)
    {
        authenticatedUser = new AuthenticatedUser
        {
            Id = id,
            UserName = user.UserName,
            Email = user.Email,
            PasswordHash = user.Password
        };
    }
}