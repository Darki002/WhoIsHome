using WhoIsHome.Services.Persons;

namespace WhoIsHome.WebApi.ModelControllers.Models;

public class PersonModel
{
    public required string? Id { get; set; }
    
    public required string DisplayName { get; set; }
    
    public required string Email { get; set; }

    public static PersonModel From(Person person)
    {
        return new PersonModel
        {
            Id = person.Id,
            DisplayName = person.DisplayName,
            Email = person.Email
        };
    }
}