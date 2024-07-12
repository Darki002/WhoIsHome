namespace WhoIsHome.Persons;

public interface IPersonService
{
    bool TryCreate(string name, string email);
}