using Galaxus.Functional;

namespace WhoIsHome.Persons;

public interface IPersonService
{
    Task<Result<Person, string>> GetAsync(string id);
    
    Task<Result<Person, string>> GetByMailAsync(string email);
    
    Task<Result<Person, string>> CreateAsync(string name, string email);
}