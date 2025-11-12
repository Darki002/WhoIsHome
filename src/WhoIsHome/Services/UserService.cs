using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WhoIsHome.Entities;
using WhoIsHome.External.Database;
using WhoIsHome.Validations;

namespace WhoIsHome.Services;

internal class UserService(WhoIsHomeContext context, IPasswordHasher<User> passwordHasher) : IUserService
{
    public async Task<User?> GetAsync(int id, CancellationToken cancellationToken)
    {
        return await context.Users
            .Where(u => u.Id == id)
            .SingleOrDefaultAsync(cancellationToken);
    }
    
    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await context.Users
            .Where(u => u.Email == email)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<ValidationResult<User>> CreateUserAsync(string userName, string email, string password, CancellationToken cancellationToken)
    {
        var result = new ValidationResult<User>();
        
        var isEmailInUse = context.Users.Any(u => u.Email == email);

        if (isEmailInUse)
        {
            result.ValidationErrors.Add(new ValidationError("Email is already in user."));
        }

        var passwordHash = passwordHasher.HashPassword(null!, password);
        var user = new User
        {
            UserName = userName,
            Email = email,
            Password = passwordHash
        };
        
        result.ValidationErrors.AddRange(user.Validate());

        if (result.HasErrors)
        {
            return result;
        }
        
        var dbUser = context.Users.Add(user);
        await context.SaveChangesAsync(cancellationToken);
        return ValidationResult<User>.Success(dbUser.Entity);
    }
}