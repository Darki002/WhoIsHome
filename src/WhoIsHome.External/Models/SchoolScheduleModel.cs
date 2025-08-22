using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.External.Models;

public class SchoolScheduleModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required] 
    public required string SchoolName { get; set; } = null!;
    
    [Required]
    public int DayOfWeek { get; set; }
    
    [Required]
    public TimeOnly StartTime { get; set; }
    
    public TimeOnly? EndTime { get; set; }
    
    [Required]
    [DefaultValue(WhoIsHome.Shared.Types.PresenceType.Unknown)]
    public PresenceType? PresenceType { get; set; }
    
    public TimeOnly? DinnerTime { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    public UserModel User { get; set; } = null!;
}