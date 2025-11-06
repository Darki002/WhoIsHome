using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace WhoIsHome.External.Models;

public class PushUpSettingsModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [MaxLength(100)]
    public string? Token { get; set; }
    
    [Required]
    public bool Enabled { get; set; }
    
    [MaxLength(10)]
    public CultureInfo? LanguageCode { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    public UserModel User { get; set; } = null!;
}