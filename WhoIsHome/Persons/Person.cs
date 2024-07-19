using Google.Cloud.Firestore;

namespace WhoIsHome.Persons;

[FirestoreData]
public class Person
{
    [FirestoreDocumentId]
    public string? Id { get; set; }
    
    
    [FirestoreProperty("displayName")]
    public string DisplayName { get; set;  } = null!;
    
    
    [FirestoreProperty("email")]
    public string Email { get; set; } = null!;
}