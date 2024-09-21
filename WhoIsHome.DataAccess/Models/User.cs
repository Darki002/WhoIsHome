using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using WhoIsHome.Shared;

namespace WhoIsHome.DataAccess.Models;

[Index(nameof(Email), IsUnique = true)]
public class User : DbModel
{
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