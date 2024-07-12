using System.Net.Mail;

namespace WhoIsHome.Persons;

public class Person
{
    public string? Id { get; private set; }
    public string Name { get; private set;  }
    public MailAddress Email { get; private set; }

    private Person(string name, MailAddress email)
    {
        Name = name;
        Email = email;
    }

    public Person? NewTry(string name, string email)
    {
        return MailAddress.TryCreate(email, out var mailAddress) 
            ? new Person(name, mailAddress) 
            : null;
    }

    public void SetId(string id)
    {
        Id = id;
    }
}