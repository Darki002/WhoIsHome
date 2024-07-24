namespace WhoIsHome.WebApi.ModelControllers.Models;

public class NewPersonModel
{
    public required string DisplayName { get; set; }
    
    public required string Email { get; set; }
}