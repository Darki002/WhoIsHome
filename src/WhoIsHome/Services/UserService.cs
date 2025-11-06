using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WhoIsHome.Entities;
using WhoIsHome.External;
using WhoIsHome.External.Models;
using WhoIsHome.Shared.Exceptions;

namespace WhoIsHome.Services;

internal class UserService(IDbContextFactory<WhoIsHomeContext> contextFactory, IPasswordHasher<User> passwordHasher) : IUserAggregateService
{
    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var user = await context.Users
            .Where(u => u.Email == email)
            .SingleOrDefaultAsync(cancellationToken);
        return new User(user);
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
        
        var user = new User(userName, email, passwordHash);
        var model = new UserModel
        {
            Id = user.Id!.Value,
            UserName = user.UserName,
            Email = user.Email,
            Password = user.Password
        };
        
        var newUser = context.Users.Add(model);
        await context.SaveChangesAsync(cancellationToken);
        return new User(newUser.Entity);
    }

    public async Task<User> GetAsync(int id, CancellationToken cancellationToken)
    {
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var userModel = await context.Users.SingleOrDefaultAsync(u =>  u.Id == id, cancellationToken);

        if (userModel is null)
        {
            throw new NotFoundException($"User with Id {id} does not exist");
        }
        
        return new User(userModel);
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