using Galaxus.Functional;
using Google.Cloud.Firestore;

namespace WhoIsHome.Services;

public interface IService<TDbModel>
{
    Task<Result<TDbModel, string>> GetAsync(string id, CancellationToken cancellationToken);

    IReadOnlyCollection<TDbModel?> QueryMany(Func<CollectionReference, QuerySnapshot> query);

    TDbModel? QuerySingle(Func<CollectionReference, DocumentSnapshot?> query);
}