using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WhoIsHome.External.Models;

[Table("Chore")]
public class ChoreModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
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