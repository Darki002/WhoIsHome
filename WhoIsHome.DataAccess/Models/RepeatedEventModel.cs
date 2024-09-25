using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WhoIsHome.Shared;
using WhoIsHome.Shared.BaseTypes;

namespace WhoIsHome.DataAccess.Models;

[Table("RepeatedEvent")]
public class RepeatedEventModel : DbModel
{
    [Required] [MaxLength(50)] public string Title { get; set; } = null!;
    
    [Required]
    public DateOnly FirstOccurrence { get; set; }
    
    [Required]
    public DateOnly LastOccurrence { get; set; }
    
    [Required]
    public TimeOnly StartTime { get; set; }
    
    [Required]
    public TimeOnly EndTime { get; set; }
    
    [Required]
    public DinnerTimeModel DinnerTimeModel { get; set; } = null!;
    
    [Required]
    public UserModel UserModel { get; set; } = null!;
}