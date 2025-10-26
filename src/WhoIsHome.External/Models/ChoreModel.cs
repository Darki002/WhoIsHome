using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WhoIsHome.Shared.BaseTypes;

namespace WhoIsHome.External.Models;

[Table("Chore")]
public class ChoreModel : DbModel
{
    [Required]
    [MaxLength(50)]
    public required string Title { get; set; }
    
    [Required]
    [MaxLength(200)]
    public required string Description { get; set; }
    
    [Required]
    public ushort Repetition { get; set; }
    
    public int? AssignedUserId { get; set; }
    
    public UserModel? AssignedUser { get; set; }
}