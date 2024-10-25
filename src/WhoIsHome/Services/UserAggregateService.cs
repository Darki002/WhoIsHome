using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WhoIsHome.Aggregates;
using WhoIsHome.Aggregates.Mappers;
using WhoIsHome.DataAccess;
using WhoIsHome.Shared.Exceptions;

namespace WhoIsHome.Services;

internal class UserAggregateService(IDbContextFactory<WhoIsHomeContext> contextFactory, IPasswordHasher<User> passwordHasher) : IUserAggregateService
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

    public async Task<User> GetAsync(int id, CancellationToken cancellationToken)
    {
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var userModel = await context.Users.SingleOrDefaultAsync(u =>  u.Id == id, cancellationToken);

        if (userModel is null)
        {
            throw new NotFoundException($"User with Id {id} does not exist");
        }
        
        return userModel.ToAggregate();
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var userModel = await context.Users.SingleOrDefaultAsync(u =>  u.Id == id, cancellationToken);

        if (userModel is null)
        {
            throw new NotFoundException($"User with Id {id} does not exist");
        }

        context.Users.Remove(userModel);
        await context.SaveChangesAsync(cancellationToken);
    }
}