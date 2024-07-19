using Google.Cloud.Firestore;

namespace WhoIsHome.Events;

public class EventService(FirestoreDb firestoreDb) : ServiceBase<Event>(firestoreDb), IEventService
{
    protected override string Collection { get; } = "event";
}