using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WhoIsHome.Shared.Helper;
using WhoIsHome.Shared.Types;
using WhoIsHome.Validations;

namespace WhoIsHome.Entities;

[Table("EventTemplate")]
public class EventGroup()
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Title { get; set; } = null!;

    [Required]
    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    [Required]
    public WeekDay WeekDays { get; set; }

    [Required]
    public TimeOnly StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }
    
    [Required]
    [DefaultValue(PresenceType.Unknown)]
    public PresenceType PresenceType { get; set; }

    public TimeOnly? DinnerTime { get; set; }

    [Required]
    public int UserId { get; set; }
    
    [Required]
    public User User { get; set; } = null!;

    public List<EventInstance> Events { get; set; } = [];
    
    public EventGroup(
        string title,
        DateOnly startDate,
        DateOnly? endDate,
        WeekDay weekDays,
        TimeOnly startTime,
        TimeOnly? endTime,
        PresenceType presenceType, 
        TimeOnly? dinnerTime,
        int userId) : this()
    {
        Title = title;
        StartDate = startDate;
        EndDate = endDate;
        WeekDays = weekDays;
        StartTime = startTime;
        EndTime = endTime;
        PresenceType = presenceType;
        DinnerTime = dinnerTime;
        UserId = userId;
    }
    
    public List<ValidationError> Validate()
    {
        List<ValidationError> validationErrors = [];
        if (StartDate > EndDate)
        {
            validationErrors.Add(new ValidationError("First occurrence must be before the last occurrence."));
        }
        if (Title.Length >= 50)
        {
            validationErrors.Add(new ValidationError("Title must be less then or equal to 50 characters long."));
        }
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