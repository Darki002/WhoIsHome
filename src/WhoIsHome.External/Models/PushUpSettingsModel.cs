using System.ComponentModel.DataAnnotations;
using WhoIsHome.Shared.BaseTypes;

namespace WhoIsHome.External.Models;

public class PushUpSettingsModel : DbModel
{
    [MaxLength(100)]
    public string? Token { get; set; } // TODO: Generate DB
    
    [Required]
    public bool Enabled { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    public UserModel User { get; set; } = null!;
}