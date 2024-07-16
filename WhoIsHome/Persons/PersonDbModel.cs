namespace WhoIsHome.Persons;

public class PersonDbModel
{
    public string Id { get; private set; } = null!;
    public string DisplayName { get; private set;  } = null!;
    public string Email { get; private set; } = null!;
}