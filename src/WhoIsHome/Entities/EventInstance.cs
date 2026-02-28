using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WhoIsHome.Shared.Types;
using WhoIsHome.Validations;

namespace WhoIsHome.Entities;

[Table("event")]
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
    public TimeOnly StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }
    
    [Required]
    [DefaultValue(PresenceType.Unknown)]
    public PresenceType PresenceType { get; set; }

    public TimeOnly? DinnerTime { get; set; }
    
    [Required]
    public bool IsOriginal { get; set; }
    
    [Required]
    public DateOnly OriginalDate { get; set; }
    
    [Required]
    public int EventGroupId { get; set; }

    [Required] 
    public EventGroup EventGroup { get; set; } = null!;
    
    [Required]
    public int UserId { get; set; }

    [Required] 
    public User User { get; set; } = null!;

    public bool IsAtHome => PresenceType != PresenceType.NotPresent;
    
    public List<ValidationError> Validate()
    {
        List<ValidationError> validationErrors = [];
        if (StartTime > EndTime)
        {
            validationErrors.Add(new ValidationError("StartDate must be before EndDate."));
        }
        if (EndTime > DinnerTime)
        {
            validationErrors.Add(new ValidationError("Dinner Time must be later then the End Time of the Event."));
        }

        ValidatePresence(validationErrors);

        return validationErrors;
    }
    
    private void ValidatePresence(List<ValidationError> validationErrors)
    {
        switch (PresenceType)
        {
            case PresenceType.Unknown:
                if (DinnerTime.HasValue) validationErrors.Add(new ValidationError("Can't set Time for Type Unknown."));
                break;
            case PresenceType.Default:
                if (!DinnerTime.HasValue) validationErrors.Add(new ValidationError("Must provide a Time for Default Type."));
                break;
            case PresenceType.Late:
                if (!DinnerTime.HasValue) validationErrors.Add(new ValidationError("Must provide a Time for Late Type."));
                break;
            case PresenceType.NotPresent:
                if (DinnerTime.HasValue) validationErrors.Add(new ValidationError("Can't set Time for Type NotPresent."));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(PresenceType), PresenceType, null);
        }
    }
}