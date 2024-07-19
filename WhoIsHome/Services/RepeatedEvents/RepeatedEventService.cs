using Galaxus.Functional;
using Google.Cloud.Firestore;
using WhoIsHome.Services.Persons;

namespace WhoIsHome.Services.RepeatedEvents;

public class RepeatedEventService(FirestoreDb firestoreDb, IPersonService personService) : ServiceBase<RepeatedEvent>(firestoreDb), IRepeatedEventService
{
    protected override string Collection { get; } = "repeated-events";
    
    public async Task<Result<RepeatedEvent, string>> CreateAsync(
        string eventName, 
        string personId, 
        DateTime startDate,
        DateTime endDate,
        DateTime startTime, 
        DateTime endTime,
        bool relevantForDinner, 
        DateTime dinnerAt, 
        CancellationToken cancellationToken)
    { 
        var person = await personService.GetAsync(personId, cancellationToken);
        if (person.IsErr) return person.Err.Unwrap();

        var newEvent = RepeatedEvent.TryCreate(
            eventName: eventName,
            person: person.Unwrap(),
            startDate: startDate,
            endDate: endDate,
            startTime: startTime,
            endTime: endTime,
            relevantForDinner: relevantForDinner,
            dinnerAt: dinnerAt);

        var docRef = await FirestoreDb.Collection(Collection).AddAsync(newEvent, cancellationToken);
        var snapshot = await docRef.GetSnapshotAsync(cancellationToken);
        return ConvertDocument(snapshot);
    }
    
    public async Task<Result<RepeatedEvent, string>> UpdateAsync(
        string id,
        string eventName, 
        DateTime startDate,
        DateTime endDate,
        DateTime startTime, 
        DateTime endTime,
        bool relevantForDinner, 
        DateTime dinnerAt, 
        CancellationToken cancellationToken)
    {
        var result = await GetAsync(id, cancellationToken);

        if (result.IsErr) return result.Err.Unwrap();

        var existingEvent = result.Unwrap();
        var updateResult = existingEvent.TryUpdate(
            eventName: eventName,
            startDate: startDate,
            endDate: endDate,
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