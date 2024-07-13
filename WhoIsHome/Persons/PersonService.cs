using System.Net.Mail;
using Galaxus.Functional;
using Google.Cloud.Firestore;

namespace WhoIsHome.Persons;

public class PersonService(FirestoreDbBuilder dbBuilder) : IPersonService
{
    private const string Collection = "person";
    
    private FirestoreDbBuilder dbBuilder = dbBuilder;

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

        var db = await dbBuilder.BuildAsync();
        
        Console.WriteLine(person);

        return person;
    }

    public async Task<Result<Person, string>> GetPersonByMailAsync(string email)
    {
        if (!MailAddress.TryCreate(email, out var mailAddress))
        {
            return "Invalid Mail Address Format.";
        }
        
        var db = await dbBuilder.BuildAsync();

        var result = await db.Collection(Collection).Where(Filter.EqualTo("email", email)).GetSnapshotAsync();
        var personDoc = result.Documents.Single();
        var person = personDoc.ConvertTo<Person>();

        if (person is null)
        {
            return $"Can't convert {personDoc} to type ${nameof(Person)}";
        }

        return person;
    }
}