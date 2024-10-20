using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using WhoIsHome.Shared.BaseTypes;

namespace WhoIsHome.DataAccess.Models;

[Table("RefreshToken")]
[Index(nameof(Token), IsUnique = true)]
public class RefreshTokenModel : DbModel
{
    [Required] 
    public string Token { get; set; } = null!;
    
    [Required]
    public DateTime Issued { get; set; }
    
    [Required]
    public UserModel UserModel { get; set; } = null!;
}