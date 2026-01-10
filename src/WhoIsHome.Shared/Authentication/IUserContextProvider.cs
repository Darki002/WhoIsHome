namespace WhoIsHome.Shared.Authentication;

public interface IUserContextProvider
{
    int UserId { get; }
    
    Task<AuthenticatedUser> GetCurrentUserAsync(CancellationToken cancellationToken = default);
    
    bool IsUserPermitted(int permittedUserId);
}