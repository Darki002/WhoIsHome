using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WhoIsHome.Aggregates;
using WhoIsHome.Aggregates.Mappers;
using WhoIsHome.DataAccess;
using WhoIsHome.Shared.Exceptions;

namespace WhoIsHome.Services;

public class UserAggregateService(WhoIsHomeContext context, IPasswordHasher<User> passwordHasher)
{
    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .Where(u => u.Email == email)
            .SingleOrDefaultAsync(cancellationToken);
        return user?.ToAggregate();
    }

    public async Task<User> CreateUserAsync(string userName, string email, string password, CancellationToken cancellationToken)
    {
        var isEmailInUse = context.Users.Any(u => u.Email == email);

        if (isEmailInUse)
        {
            throw new EmailInUseException();
        }

        var passwordHash = passwordHasher.HashPassword(null!, password);
        
        var user = User.Create(userName, email, passwordHash);
        var model = user.ToModel();
        context.Users.Add(model);
        await context.SaveChangesAsync(cancellationToken);

        var createdUser = await context.Users.SingleAsync(u => u.Email == email, cancellationToken);
        return createdUser.ToAggregate();
    }
}