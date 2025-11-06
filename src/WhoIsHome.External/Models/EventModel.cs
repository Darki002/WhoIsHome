using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.External.Models;

[Table("Event")]
public class EventModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Title { get; set; } = null!;
    
    [Required]
    public DateOnly Date { get; set; }
    
    [Required]
    public TimeOnly StartTime { get; set; }
    
    public TimeOnly? EndTime { get; set; }

    [Required]
    [DefaultValue(PresenceType.Unknown)]
    public PresenceType PresenceType { get; set; }
    
    public TimeOnly? DinnerTime { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    public UserModel User { get; set; } = null!;
}