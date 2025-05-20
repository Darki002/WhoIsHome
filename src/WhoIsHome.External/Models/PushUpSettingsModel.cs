using System.ComponentModel.DataAnnotations;
using System.Globalization;
using WhoIsHome.Shared.BaseTypes;

namespace WhoIsHome.External.Models;

public class PushUpSettingsModel : DbModel
{
    [MaxLength(100)]
    public string? Token { get; set; }
    
    [Required]
    public bool Enabled { get; set; }
    
    [Required]
    [MaxLength(10)]
    public required CultureInfo LanguageCode { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    public UserModel User { get; set; } = null!;
}