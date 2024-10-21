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
    [MaxLength(100)] // TODO How long is a token???
    public string Token { get; set; } = null!;
    
    [Required]
    public DateTime Issued { get; set; }
    
    [Required]
    public DateTime? ExpiredAt { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    public UserModel User { get; set; } = null!;
}