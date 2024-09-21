using System.ComponentModel.DataAnnotations;

namespace WhoIsHome.DataAccess.Models;

public class RepeatedEvent
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Title { get; set; }
    
    [Required]
    public DateOnly FirstOccurrence { get; set; }
    
    [Required]
    public DateOnly LastOccurrence { get; set; }
    
    [Required]
    public TimeOnly StartTime { get; set; }
    
    [Required]
    public TimeOnly EndTime { get; set; }
    
    [Required]
    public DinnerTime DinnerTime { get; set; }
    
    [Required]
    public User User { get; set; }
}