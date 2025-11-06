using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WhoIsHome.Entities;
using WhoIsHome.External;
using WhoIsHome.External.Models;
using WhoIsHome.Validations;

namespace WhoIsHome.Services;

internal class UserService(IDbContextFactory<WhoIsHomeContext> contextFactory, IPasswordHasher<User> passwordHasher) : IUserService
{
    public async Task<User?> GetAsync(int id, CancellationToken cancellationToken)
    {
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var user = await context.Users
            .Where(u => u.Id == id)
            .SingleOrDefaultAsync(cancellationToken);
        return user is not null ? new User(user) : null;
    }
    
    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var user = await context.Users
            .Where(u => u.Email == email)
            .SingleOrDefaultAsync(cancellationToken);
        return user is not null ? new User(user) : null;
    }

    public async Task<UserValidationResult> CreateUserAsync(string userName, string email, string password, CancellationToken cancellationToken)
    {
        var result = new UserValidationResult();
        
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var isEmailInUse = context.Users.Any(u => u.Email == email);

        if (isEmailInUse)
        {
            result.ValidationErrors.Add(new ValidationError("Email is already in user."));
        }

        var passwordHash = passwordHasher.HashPassword(null!, password);
        var user = new User(userName, email, passwordHash);
        
        result.ValidationErrors.AddRange(user.Validate());

        if (result.HasErrors)
        {
            return result;
        }
        
        var model = new UserModel
        {
            Id = user.Id!.Value,
            UserName = user.UserName,
            Email = user.Email,
            Password = user.Password
        };
        
        var newUser = context.Users.Add(model);
        await context.SaveChangesAsync(cancellationToken);
        result.User = new User(newUser.Entity);
        return result;
    }
}