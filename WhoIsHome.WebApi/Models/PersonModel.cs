namespace WhoIsHome.WebApi.Models;

public class PersonModel
{
    public required string Id { get; set; }
    
    public required string DisplayName { get; set; }
    
    public required string Email { get; set; }
}