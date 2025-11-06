using System.Net.Mail;
using WhoIsHome.External.Models;
using WhoIsHome.Validations;

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

    public List<ValidationError> Validate()
    {
        List<ValidationError> validationErrors = [];
        if (!MailAddress.TryCreate(Email, out _))
        {
            validationErrors.Add(new ValidationError("Email is not in a valid format."));
        }

        if (IsValidUserName(UserName) is false)
        {
            validationErrors.Add(new ValidationError($"UserName is to long or to short. Must be between {UserNameMinLength} and {UserNameMaxLength} Characters."));
        }

        return validationErrors;
    }

    private static bool IsValidUserName(string userName)
    {
        return userName.Length is <= UserNameMaxLength and >= UserNameMinLength;
    }
}
