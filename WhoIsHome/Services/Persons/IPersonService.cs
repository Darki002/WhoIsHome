using Galaxus.Functional;

namespace WhoIsHome.Services.Persons;

public interface IPersonService
{
    Task<Result<Person, string>> GetAsync(string id, CancellationToken cancellationToken);
    
    Task<Result<Person, string>> GetByMailAsync(string email, CancellationToken cancellationToken);
    
    Task<Result<Person, string>> CreateAsync(string name, string email, CancellationToken cancellationToken);
}