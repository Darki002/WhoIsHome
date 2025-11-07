using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.Entities;

[Table("Event")]
public class EventInstance
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public required string Title { get; set; }
    
    [Required]
    public DateOnly Date { get; set; }
    
    [Required]
    public DateOnly OriginalDate { get; set; }
    
    [Required]
    public TimeOnly StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }
    
    [Required]
    [DefaultValue(PresenceType.Unknown)]
    public PresenceType PresenceType { get; set; }

    public TimeOnly? DinnerTime { get; set; }
    
    [Required]
    public int EventTemplateId { get; set; }

    [Required] 
    public EventTemplate EventTemplate { get; set; } = null!;
    
    [Required]
    public int UserId { get; set; }

    [Required] 
    public User User { get; set; } = null!;

    public bool IsAtHome => PresenceType != PresenceType.NotPresent;
}