using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WhoIsHome.External.Models;

[Table("User")]
[Index(nameof(Email), IsUnique = true)]
public class UserModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(30)]
    public string UserName { get; set; } = null!;
    
    [Required]
    [MaxLength(30)]
    public string Email { get; set; } = null!;
    
    [Required]
    [MaxLength(100)]
    public string Password { get; set; } = null!;
}