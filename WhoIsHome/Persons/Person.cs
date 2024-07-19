using System.Net.Mail;
using Galaxus.Functional;
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

    public static Result<Person, string> TryCreate(string displayName, string email)
    {
        if (displayName.Length is 0 or > 30)
        {
            return "Name must be between 1 and 30 Characters Long.";
        }

        if (!MailAddress.TryCreate(email, out _))
        {
            return "Invalid Mail Address Format.";
        }
        
        return new Person
        {
            Id = null,
            DisplayName = displayName,
            Email = email
        };
    }
}