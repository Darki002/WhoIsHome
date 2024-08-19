using Galaxus.Functional;
using Google.Cloud.Firestore;

namespace WhoIsHome.Services;

public abstract class ServiceBase<TDbModel>(FirestoreDb firestoreDb) : IService<TDbModel> where TDbModel : class
{
    protected readonly FirestoreDb FirestoreDb = firestoreDb;

    protected abstract string Collection { get; }

    public async Task<Result<TDbModel, string>> GetAsync(string id, CancellationToken cancellationToken)
    {
        var result = await FirestoreDb.Collection(Collection)
            .Document(id)
            .GetSnapshotAsync(cancellationToken);

        return result is null
            ? $"Can't find {typeof(TDbModel).Name} with Id {id}"
            : ConvertDocument(result);
    }
    
    public async Task<Result<Unit, string>> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        _ = await FirestoreDb.Collection(Collection)
            .Document(id)
            .DeleteAsync(cancellationToken: cancellationToken);
        
        return Unit.Value;
    }

    public async Task<IReadOnlyCollection<TDbModel>> QueryManyAsync(CancellationToken cancellationToken,
        Func<CollectionReference, Task<QuerySnapshot>> query)
    {
        var collectionRef = FirestoreDb.Collection(Collection);
        var snapshots = await query(collectionRef);

        var result = ConvertAllDocument(snapshots);
        
        if (result.IsErr)
        {
            throw new InvalidOperationException(result.Err.Unwrap());
        }
        
        return result.Unwrap();
    }

    public async Task<TDbModel> QuerySingleAsync(CancellationToken cancellationToken,
        Func<CollectionReference, Task<QuerySnapshot>> query)
    {
        var collectionRef = FirestoreDb.Collection(Collection);
        var snapshot = await query(collectionRef);

        var model = ConvertDocument(snapshot.Single());
        if (model.IsErr)
        {
            throw new InvalidOperationException(model.Err.Unwrap());
        }

        return model.Unwrap();
    }

    public async Task<Result<IReadOnlyList<TDbModel>, string>> GetByPersonIdAsync(string personId, CancellationToken cancellationToken)
    {
        var snapshot = await FirestoreDb.Collection(Collection)
            .WherePersonIs(personId)
            .GetSnapshotAsync(cancellationToken);

        var result = ConvertAllDocument(snapshot);
        return result.IsErr ? result.Err.Unwrap() : result;
    }

    protected Result<TDbModel, string> ConvertDocument(DocumentSnapshot documentSnapshot)
    {
        var dbModel = documentSnapshot.ConvertTo<TDbModel>();

        if (dbModel is null)
        {
            return $"Can't convert DB Document to type ${typeof(TDbModel).Name}";
        }

        return dbModel;
    }
    
    protected Result<IReadOnlyList<TDbModel>, string> ConvertAllDocument(IReadOnlyList<DocumentSnapshot> documentSnapshots)
    {
        var models = documentSnapshots
            .Select(d => d.ConvertTo<TDbModel>())
            .ToList();

        if (models.Any(m => m is null))
        {
            return $"Can't convert DB Document to type ${typeof(TDbModel).Name}";
        }

        return models;
    }
}