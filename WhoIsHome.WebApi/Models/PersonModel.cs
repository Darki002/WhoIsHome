using WhoIsHome.Persons;

namespace WhoIsHome.WebApi.Models;

public class PersonModel
{
    public required string Id { get; set; }
    
    public required string DisplayName { get; set; }
    
    public required string Email { get; set; }

    public static PersonModel From(Person person)
    {
        return new PersonModel
        {
            Id = person.Id ?? string.Empty,
            DisplayName = person.DisplayName,
            Email = person.Email
        };
    }
}