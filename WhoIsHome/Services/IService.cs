using Galaxus.Functional;
using Google.Cloud.Firestore;

namespace WhoIsHome.Services;

public interface IService<TDbModel>
{
    Task<Result<TDbModel, string>> GetAsync(string id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<TDbModel?>> QueryManyAsync(CancellationToken cancellationToken,
        Func<CollectionReference, Task<QuerySnapshot>> query);

    Task<TDbModel?> QuerySingleAsync(CancellationToken cancellationToken,
        Func<CollectionReference, Task<QuerySnapshot>> query);
}