using System.Net.Mail;
using Galaxus.Functional;
using Google.Cloud.Firestore;

namespace WhoIsHome.Persons;

public class PersonService(FirestoreDb firestoreDb) : ServiceBase<Person>(firestoreDb), IPersonService
{
    protected override string Collection { get; } = "person";

    public async Task<Result<Person, string>> GetByMailAsync(string email)
    {
        if (!MailAddress.TryCreate(email, out _))
        {
            return "Invalid Mail Address Format.";
        }

        var result = await FirestoreDb.Collection(Collection)
            .WhereEqualTo("email", email)
            .GetSnapshotAsync();

        var personDoc = result.Documents.SingleOrDefault();
        
        return personDoc is null 
            ? $"Can't find {nameof(Person)} with Email {email}" 
            : ConvertDocument(personDoc);
    }
    
    public async Task<Result<Person, string>> CreateAsync(string name, string email)
    {
        var person = Person.TryCreate(name, email);
        if (person.IsErr) return person.Err.Unwrap();

        var existingPerson = await FirestoreDb
            .Collection(Collection)
            .WhereEqualTo("email", email)
            .Count()
            .GetSnapshotAsync();

        if (existingPerson.Count > 0)
        {
            return $"Person with email {email} already exists";
        }

        var docRef = await FirestoreDb.Collection(Collection).AddAsync(person.Unwrap());
        var personDoc = await docRef.GetSnapshotAsync();
        return ConvertDocument(personDoc);
    }
}