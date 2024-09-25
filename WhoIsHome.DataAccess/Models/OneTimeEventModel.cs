using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WhoIsHome.Shared;
using WhoIsHome.Shared.BaseTypes;

namespace WhoIsHome.DataAccess.Models;

[Table("Event")]
public class OneTimeEventModel : DbModel
{
    [Required]
    [MaxLength(50)]
    public string Title { get; set; } = null!;
    
    [Required]
    public DateOnly Date { get; set; }
    
    [Required]
    public TimeOnly StartTime { get; set; }
    
    [Required]
    public TimeOnly EndTime { get; set; }

    [Required] public DinnerTimeModel DinnerTimeModel { get; set; } = null!;
    
    [Required]
    public UserModel UserModel { get; set; } = null!;
}