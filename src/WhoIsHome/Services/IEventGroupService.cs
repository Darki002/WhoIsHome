using WhoIsHome.Entities;
using WhoIsHome.Shared.Types;
using WhoIsHome.Validations;

namespace WhoIsHome.Services;

public interface IEventGroupService
{
    Task<ValidationResult<EventGroup>> GetAsync(int id, CancellationToken cancellationToken);
    
    Task<EventGroup> CreateAsync(
        string title, 
        DateOnly startDate, 
        DateOnly? endDate, 
        WeekDay weekDays, 
        TimeOnly startTime, 
        TimeOnly? endTime,
        PresenceType presenceType, 
        TimeOnly? dinnerTime, 
        CancellationToken cancellationToken);

    Task<ValidationResult<EventGroup>> UpdateAsync(
        int id, 
        string title, 
        DateOnly startDate, 
        DateOnly? endDate, 
        WeekDay weekDays, 
        TimeOnly startTime, 
        TimeOnly? endTime,
        PresenceType presenceType, 
        TimeOnly? dinnerTime, 
        CancellationToken cancellationToken);

    Task<ValidationError?> DeleteAsync(int id, CancellationToken cancellationToken);
}