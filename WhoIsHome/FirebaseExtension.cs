using Google.Cloud.Firestore;
using WhoIsHome.Services.Persons;

namespace WhoIsHome;

public static class FirebaseExtension
{
    public static Query WherePersonIs(this Query query, Person person)
    {
        return query.WhereEqualTo("person:id", person.Id);
    }
    
    public static Query WherePersonIs(this Query query, string personId)
    {
        return query.WhereEqualTo("person:id", personId);
    }
}