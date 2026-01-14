using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using WhoIsHome.Validations;

namespace WhoIsHome.Entities;

[Table("user")]
[Index(nameof(Email), IsUnique = true)]
public class User
{
    private const int UserNameMinLength = 1;
    private const int UserNameMaxLength = 30;

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(UserNameMaxLength)]
    public required string UserName { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Email { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Password { get; set; }

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
