using Galaxus.Functional;

namespace WhoIsHome.Services;

public interface IService<TDbModel>
{
    Task<Result<TDbModel, string>> GetAsync(string id, CancellationToken cancellationToken);
}