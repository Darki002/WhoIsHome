using Microsoft.EntityFrameworkCore;
using WhoIsHome.Aggregates;
using WhoIsHome.DataAccess;
using WhoIsHome.DataAccess.Models;

namespace WhoIsHome.Services;

public class UserService(WhoIsHomeContext context)
{
    public async Task<User> GetUserByIdAsync(int id, CancellationToken cancellationToken)
    {
        var user = await context.Users.SingleAsync(u => u.Id == id, cancellationToken);
        return user.ToAggregate<User>();
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