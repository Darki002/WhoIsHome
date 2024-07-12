using System.Net.Mail;
using Galaxus.Functional;

namespace WhoIsHome.Persons;

public class PersonService : IPersonService
{
    public Result<Person, string> TryCreate(string name, string email)
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

        // TODO save to DB
        Console.WriteLine(person);

        return person;
    }
}