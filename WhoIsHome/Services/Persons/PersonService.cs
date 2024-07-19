using System.Net.Mail;
using Galaxus.Functional;
using Google.Cloud.Firestore;

namespace WhoIsHome.Services.Persons;

public class PersonService(FirestoreDb firestoreDb) : ServiceBase<Person>(firestoreDb), IPersonService
{
    protected override string Collection { get; } = "person";

    public async Task<Result<IReadOnlyCollection<Person>, string>> GetAllAsync(CancellationToken cancellationToken)
    {
        var query = await FirestoreDb.Collection(Collection)
            .GetSnapshotAsync(cancellationToken);

        var result = new List<Person>();
        
        foreach (var snapshot in query)
        {
            var model = ConvertDocument(snapshot);
            if (model.IsErr)
            {
                return model.Err.Unwrap();
            }
            result.Add(model.Unwrap());
        }

        return result;
    }

    public async Task<Result<Person, string>> GetByMailAsync(string email, CancellationToken cancellationToken)
    {
        if (!MailAddress.TryCreate(email, out _))
        {
            return "Invalid Mail Address Format.";
        }

        var result = await FirestoreDb.Collection(Collection)
            .WhereEqualTo("email", email)
            .GetSnapshotAsync(cancellationToken);

        var personDoc = result.Documents.SingleOrDefault();
        
        return personDoc is null 
            ? $"Can't find {nameof(Person)} with Email {email}" 
            : ConvertDocument(personDoc);
    }
    
    public async Task<Result<Person, string>> CreateAsync(string name, string email, CancellationToken cancellationToken)
    {
        var person = Person.TryCreate(name, email);
        if (person.IsErr) return person.Err.Unwrap();

        var existingPerson = await FirestoreDb
            .Collection(Collection)
            .WhereEqualTo("email", email)
            .Count()
            .GetSnapshotAsync(cancellationToken);

        if (existingPerson.Count > 0)
        {
            return $"Person with email {email} already exists";
        }

        var docRef = await FirestoreDb.Collection(Collection).AddAsync(person.Unwrap(), cancellationToken);
        var personDoc = await docRef.GetSnapshotAsync(cancellationToken);
        return ConvertDocument(personDoc);
    }
}