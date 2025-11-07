using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using WhoIsHome.Entities;

namespace WhoIsHome.External.PushUp;

[Table("PushUpSetting")]
public class PushUpSettings
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
    public User User { get; set; } = null!;
}