using Galaxus.Functional;
using Google.Cloud.Firestore;
using WhoIsHome.Services.Persons;

namespace WhoIsHome.Services.Events;

public class EventService(FirestoreDb firestoreDb, IPersonService personService) : ServiceBase<Event>(firestoreDb), IEventService
{
    protected override string Collection { get; } = "event";

    public async Task<Result<Event, string>> CreateAsync(
        string eventName, 
        string personId, 
        DateOnly date,
        TimeOnly startTime, 
        TimeOnly endTime,
        bool relevantForDinner, 
        TimeOnly? dinnerAt, 
        CancellationToken cancellationToken)
    { 
        var person = await personService.GetAsync(personId, cancellationToken);
        if (person.IsErr) return person.Err.Unwrap();

        var newEvent = Event.TryCreate(
            eventName: eventName,
            person: person.Unwrap(),
            date: date,
            startTime: startTime,
            endTime: endTime,
            relevantForDinner: relevantForDinner,
            dinnerAt: dinnerAt);

        var docRef = await FirestoreDb.Collection(Collection).AddAsync(newEvent, cancellationToken);
        var snapshot = await docRef.GetSnapshotAsync(cancellationToken);
        return ConvertDocument(snapshot);
    }
    
    public async Task<Result<Event, string>> UpdateAsync(
        string id,
        string eventName, 
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        bool relevantForDinner,
        TimeOnly? dinnerAt,
        CancellationToken cancellationToken)
    {
        var result = await GetAsync(id, cancellationToken);

        if (result.IsErr) return result.Err.Unwrap();

        var existingEvent = result.Unwrap();
        var updateResult = existingEvent.TryUpdate(
            eventName: eventName,
            date: date,
            startTime: startTime,
            endTime: endTime,
            relevantForDinner: relevantForDinner,
            dinnerAt: dinnerAt);

        if (updateResult.IsErr) return updateResult.Err.Unwrap();

        var docRef = FirestoreDb.Collection(Collection).Document(id)!;
        await docRef.UpdateAsync(updateResult.Unwrap(), cancellationToken: cancellationToken);
        var snapshot = await docRef.GetSnapshotAsync(cancellationToken);
        return ConvertDocument(snapshot);
    }
}