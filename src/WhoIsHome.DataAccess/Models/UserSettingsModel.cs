using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WhoIsHome.Shared.BaseTypes;

namespace WhoIsHome.DataAccess.Models;

[Table("UserSettings")]
public class UserSettingsModel : DbModel
{
    public TimeOnly? DefaultDinnerTime { get; set; }
    
    [Required]
    public int UserId { get; set; }

    [Required] 
    public UserModel User { get; set; } = null!;
}