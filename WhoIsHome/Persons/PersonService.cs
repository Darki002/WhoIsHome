using System.Net.Mail;
using Galaxus.Functional;
using Google.Cloud.Firestore;

namespace WhoIsHome.Persons;

public class PersonService(FirestoreDb firestoreDb) : IPersonService
{
    private const string Collection = "person";

    public async Task<Result<Person, string>> TryCreateAsync(string name, string email)
    {
        if (name.Length is 0 or > 30)
        {
            return "Name must be between 1 and 30 Characters Long.";
        }
        
        if (!MailAddress.TryCreate(email, out var mailAddress))
        {
            return "Invalid Mail Address Format.";
        }
        
        var person = new Person(name, mailAddress);
        Console.WriteLine(person);

        return person;
    }

    public async Task<Result<Person, string>> GetPersonByMailAsync(string email)
    {
        if (!MailAddress.TryCreate(email, out _))
        {
            return "Invalid Mail Address Format.";
        }

        var result = await firestoreDb.Collection(Collection).Where(Filter.EqualTo("email", email)).GetSnapshotAsync();
        var personDoc = result.Documents.Single();
        var personDbModel = personDoc.ConvertTo<PersonDbModel>();

        if (personDbModel is null)
        {
            return $"Can't convert {personDoc} to type ${nameof(PersonDbModel)}";
        }

        return Person.FromDb(personDbModel);
    }
}