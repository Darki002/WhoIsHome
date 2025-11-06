using WhoIsHome.Aggregates;
using WhoIsHome.Entities;

namespace WhoIsHome.Services;

public interface IUserAggregateService : IAggregateService<User>
{
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);

    Task<User> CreateUserAsync(string userName, string email, string password, CancellationToken cancellationToken);
}