using System.Net.Mail;
using WhoIsHome.External.Models;
using WhoIsHome.Shared.Exceptions;

namespace WhoIsHome.Entities;

public class User(int? id, string userName, string email, string password)
{
    private const int UserNameMinLength = 1;
    private const int UserNameMaxLength = 30;

    public int? Id { get; } = id;

    public string UserName { get; private set; } = userName;

    public string Email { get; } = email;

    public string Password { get; private set; } = password;

    public User(string userName, string email, string password) : this(null, userName, email, password) { }
    
    public User(UserModel user) : this(user.Id, user.UserName, user.Email, user.Password) { }

    public void Validate()
    {
        _ = new MailAddress(Email);
        
        if (IsValidUserName(UserName) is false)
            throw new InvalidModelException($"UserName is to long or to short. Must be between {UserNameMinLength} and {UserNameMaxLength} Characters.");
    }

    private static bool IsValidUserName(string userName)
    {
        return userName.Length is <= UserNameMaxLength and >= UserNameMinLength;
    }
}
