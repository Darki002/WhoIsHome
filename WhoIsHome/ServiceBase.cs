using Galaxus.Functional;
using Google.Cloud.Firestore;

namespace WhoIsHome;

public abstract class ServiceBase<TDbModel>(FirestoreDb firestoreDb) where TDbModel : class
{
    protected readonly FirestoreDb FirestoreDb = firestoreDb;
    
    protected abstract string Collection { get; }
    
    public async Task<Result<TDbModel, string>> GetAsync(string id)
    {
        var result = await FirestoreDb.Collection(Collection)
            .Document(id)
            .GetSnapshotAsync();

        return result is null 
            ? $"Can't find {typeof(TDbModel).Name} with Id {id}" 
            : ConvertDocument(result);
    }

    protected Result<TDbModel, string> ConvertDocument(DocumentSnapshot documentSnapshot)
    {
        var dbModel = documentSnapshot.ConvertTo<TDbModel>();

        if (dbModel is null)
        {
            return $"Can't convert {documentSnapshot} to type ${typeof(TDbModel).Name}";
        }

        return dbModel;
    }
}