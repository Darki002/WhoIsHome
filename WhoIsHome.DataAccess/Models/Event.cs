using System.ComponentModel.DataAnnotations;

namespace WhoIsHome.DataAccess.Models;

public class Event
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Title { get; set; } = null!;
    
    [Required]
    public DateOnly Date { get; set; }
    
    [Required]
    public TimeOnly StartTime { get; set; }
    
    [Required]
    public TimeOnly EndTime { get; set; }

    [Required] public DinnerTime DinnerTime { get; set; } = null!;
    
    [Required]
    public User User { get; set; } = null!;
}