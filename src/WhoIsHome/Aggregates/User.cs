using System.Net.Mail;
using WhoIsHome.Shared.Authentication;
using WhoIsHome.Shared.BaseTypes;
using WhoIsHome.Shared.Exceptions;
using ZstdSharp.Unsafe;

namespace WhoIsHome.Aggregates;

public class User : AggregateBase
{
    private const int UserNameMinLength = 1;
    private const int UserNameMaxLength = 30;

    public int? Id { get; }

    public string UserName { get; private set; }

    public string Email { get; }

    public string Password { get; private set; }

    public User(int? id, string userName, string email, string password)
    {
        Id = id;
        UserName = userName;
        Email = email;
        Password = password;
    }
    
    public static User Create(string userName, string email, string passwordHash)
    {
        // validates Email, throws if Invalid format
        _ = new MailAddress(email);
        
        if (IsValidUserName(userName) is false)
            throw new InvalidModelException($"UserName is to long or to short. Must be between {UserNameMinLength} and {UserNameMaxLength} Characters.");
        
        return new User(
            null,
            userName,
            email,
            passwordHash);
    }

    public static User FromAuthenticatedUser(AuthenticatedUser authenticatedUser)
    {
        return new User(authenticatedUser.Id,
            authenticatedUser.UserName,
            authenticatedUser.Email,
            authenticatedUser.PasswordHash);
    }

    private static bool IsValidUserName(string userName)
    {
        return userName.Length is <= UserNameMaxLength and >= UserNameMinLength;
    }

    public override bool Equals(object? obj)
    {
        if (obj is User user)
        {
            return Equals(user);
        }

        return false;
    }

    private bool Equals(User other)
    {
        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}

public static class UserExtension
{
    public static User ToUser(this AuthenticatedUser authenticatedUser)
    {
        return User.FromAuthenticatedUser(authenticatedUser);
    }
}