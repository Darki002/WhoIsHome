using Google.Cloud.Firestore;
using WhoIsHome.Services.Persons;

namespace WhoIsHome;

public static class FirebaseExtension
{
    public static Query WherePersonIs(this Query query, string personId)
    {
        return query.WhereEqualTo("person:id", personId);
    }
}