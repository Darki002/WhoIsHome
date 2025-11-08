using WhoIsHome.Entities;
using WhoIsHome.Shared.Types;
using WhoIsHome.Validations;

namespace WhoIsHome.Services;

public interface IEventGroupService
{
    Task<EventGroup> CreateAsync(string title, DateOnly firstOccurrence, DateOnly? lastOccurrence,
        TimeOnly startTime, TimeOnly? endTime, WeekDay weekDays, PresenceType presenceType, TimeOnly? time,
        CancellationToken cancellationToken);

    Task<ValidationResult<EventGroup>> UpdateAsync(int id, string title, DateOnly startDate,
        DateOnly? endDate, TimeOnly startTime, TimeOnly? endTime, WeekDay weekDays, PresenceType presenceType, TimeOnly? time,
        CancellationToken cancellationToken);

    Task<ValidationError?> DeleteAsync(int id, CancellationToken cancellationToken);
}