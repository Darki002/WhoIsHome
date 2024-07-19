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

    public IReadOnlyCollection<TDbModel?> QueryMany(Func<CollectionReference, QuerySnapshot> query)
    {
        var collectionRef = FirestoreDb.Collection(Collection);
        var snapshots = query(collectionRef);
        
        var result = new List<TDbModel?>();
        
        foreach (var snapshot in snapshots)
        {
            if (snapshot is null)
            {
                continue;
            }
            
            var model = ConvertDocument(snapshot);
            if (model.IsErr)
            {
                throw new InvalidOperationException(model.Err.Unwrap());
            }
            result.Add(model.Unwrap());
        }

        return result;
    }
    
    public TDbModel? QuerySingle(Func<CollectionReference, DocumentSnapshot?> query)
    {
        var collectionRef = FirestoreDb.Collection(Collection);
        var snapshot = query(collectionRef);

        if (snapshot is null) return null;

        var model = ConvertDocument(snapshot);
        if (model.IsErr)
        {
            throw new InvalidOperationException(model.Err.Unwrap());
        }

        return model.Unwrap();
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
}