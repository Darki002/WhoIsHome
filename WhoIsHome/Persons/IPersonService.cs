using Galaxus.Functional;

namespace WhoIsHome.Persons;

public interface IPersonService
{
    Task<Result<Person, string>> GetPersonAsync(string id);
    
    Task<Result<Person, string>> GetPersonByMailAsync(string email);
    
    Task<Result<Person, string>> TryCreateAsync(string name, string email);
}