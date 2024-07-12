using Galaxus.Functional;

namespace WhoIsHome.Persons;

public interface IPersonService
{
    Result<Person, string> TryCreate(string name, string email);
}