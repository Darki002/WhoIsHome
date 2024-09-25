using Microsoft.EntityFrameworkCore;
using WhoIsHome.Aggregates;
using WhoIsHome.DataAccess;
using WhoIsHome.DataAccess.Models;
using WhoIsHome.Shared.Exceptions;

namespace WhoIsHome.Services;

public class UserAggregateService(WhoIsHomeContext context)
{
    public async Task<User> GetUserByIdAsync(int id, CancellationToken cancellationToken)
    {
        var user = await context.Users.SingleOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException($"No User found with id {id}");
        }
        
        return user.ToAggregate<User>();
    }
    
    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .Where(u => u.Email == email)
            .SingleOrDefaultAsync(cancellationToken);
        return user?.ToAggregate<User>();
    }

    public async Task<User> CreateUserAsync(string userName, string email, string password, CancellationToken cancellationToken)
    {
        var user = User.Create(userName, email, password);
        var model = user.ToDbModel<UserModel>();
        context.Users.Add(model);
        await context.SaveChangesAsync(cancellationToken);

        var createdUser = await context.Users.SingleAsync(u => u.Email == email, cancellationToken);
        return createdUser.ToAggregate<User>();
    }
}