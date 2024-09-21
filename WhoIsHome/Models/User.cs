using System.Text.RegularExpressions;

namespace WhoIsHome.Models;

public partial class User
{
    private const int UserNameMinLength = 5;
    private const int UserNameMaxLength = 30;

    private User(int? id, string userName, string email, string passwordHash)
    {
        Id = id;
        UserName = userName;
        Email = email;
        PasswordHash = passwordHash;
    }

    public int? Id { get; }

    public string UserName { get; private set; }

    public string Email { get; }

    public string PasswordHash { get; private set; }

    public static User Create(string userName, string email, string passwordHash)
    {
        if (IsValidEmail(email) is false)
            throw new ArgumentException("Email is not in the correct format.", nameof(email));

        if (IsValidUserName(userName))
            throw new ArgumentException(
                $"UserName is to long or to short. Must be between {UserNameMinLength} and {UserNameMaxLength} Characters.",
                nameof(userName));
        
        return new User(
            null,
            userName,
            email,
            passwordHash);
    }

    private static bool IsValidUserName(string userName)
    {
        return userName.Length is > UserNameMaxLength or < UserNameMinLength;
    }

    private static bool IsValidEmail(string email)
    {
        return MyRegex().IsMatch(email);
    }

    [GeneratedRegex(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$")]
    private static partial Regex MyRegex();
}