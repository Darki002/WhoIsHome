using WhoIsHome.Entities;
using WhoIsHome.Validations;

namespace WhoIsHome.Services;

public interface IUserService
{
    Task<User?> GetAsync(int id, CancellationToken cancellationToken);
    
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);

    Task<ValidationResult<User>> CreateUserAsync(string userName, string email, string password, CancellationToken cancellationToken);
}