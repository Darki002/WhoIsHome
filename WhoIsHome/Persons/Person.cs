using System.Net.Mail;

namespace WhoIsHome.Persons;

public class Person
{
    public string? Id { get; private set; }
    public string Name { get; private set;  }
    public MailAddress Email { get; private set; }

    internal Person(string name, MailAddress email)
    {
        Name = name;
        Email = email;
    }

    public void SetId(string id)
    {
        Id = id;
    }
}