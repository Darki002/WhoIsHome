using Galaxus.Functional;
using Google.Cloud.Firestore;

namespace WhoIsHome;

public abstract class ServiceBase<T>(FirestoreDb firestoreDb) where T : class
{
    protected readonly FirestoreDb FirestoreDb = firestoreDb;
    
    protected abstract string Collection { get; }
    
    public async Task<Result<T, string>> GetAsync(string id)
    {
        var result = await FirestoreDb.Collection(Collection)
            .Document(id)
            .GetSnapshotAsync();

        return result is null 
            ? $"Can't find {typeof(T).Name} with Id {id}" 
            : ConvertDocument(result);
    }

    protected Result<T, string> ConvertDocument(DocumentSnapshot documentSnapshot)
    {
        var dbModel = documentSnapshot.ConvertTo<T>();

        if (dbModel is null)
        {
            return $"Can't convert {documentSnapshot} to type ${typeof(T).Name}";
        }

        return dbModel;
    }
}