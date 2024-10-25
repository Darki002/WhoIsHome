using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WhoIsHome.Aggregates;
using WhoIsHome.Aggregates.Mappers;
using WhoIsHome.DataAccess;
using WhoIsHome.Shared.Exceptions;

namespace WhoIsHome.Services;

public class UserAggregateService(IDbContextFactory<WhoIsHomeContext> contextFactory, IPasswordHasher<User> passwordHasher)
{
    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var user = await context.Users
            .Where(u => u.Email == email)
            .SingleOrDefaultAsync(cancellationToken);
        return user?.ToAggregate();
    }

    public async Task<User> CreateUserAsync(string userName, string email, string password, CancellationToken cancellationToken)
    {
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
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

    public async Task<User> GetUserAsync(int userId, CancellationToken cancellationToken)
    {
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var userModel = await context.Users.SingleOrDefaultAsync(u =>  u.Id == userId, cancellationToken);

        if (userModel is null)
        {
            throw new NotFoundException($"User with Id {userId} does not exist");
        }
        
        return userModel.ToAggregate();
    }
}