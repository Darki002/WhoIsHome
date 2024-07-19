using Galaxus.Functional;

namespace WhoIsHome.Services.Persons;

public interface IPersonService : IService<Person>
{
    Task<Result<Person, string>> GetByMailAsync(string email, CancellationToken cancellationToken);
    
    Task<Result<Person, string>> CreateAsync(string name, string email, CancellationToken cancellationToken);
}