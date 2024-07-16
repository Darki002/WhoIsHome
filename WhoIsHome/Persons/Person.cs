using System.Net.Mail;

namespace WhoIsHome.Persons;

public class Person
{
    public string? Id { get; private set; }
    public string DisplayName { get; private set;  }
    public MailAddress Email { get; private set; }

    private Person() {}
    
    internal Person(string name, MailAddress email)
    {
        DisplayName = name;
        Email = email;
    }

    public static Person FromDb(PersonDbModel dbModel)
    {
        var mailAddress = new MailAddress(dbModel.Email);

        return new Person
        {
            Id = dbModel.Id,
            DisplayName = dbModel.DisplayName,
            Email = mailAddress
        };
    }
}