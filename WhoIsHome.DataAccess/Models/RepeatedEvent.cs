using System.ComponentModel.DataAnnotations;
using WhoIsHome.Shared;

namespace WhoIsHome.DataAccess.Models;

public class RepeatedEvent : DbModel
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
    public DinnerTime DinnerTime { get; set; } = null!;
    
    [Required]
    public User User { get; set; } = null!;
}