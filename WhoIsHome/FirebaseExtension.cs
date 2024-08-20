using Google.Cloud.Firestore;

namespace WhoIsHome;

public static class FirebaseExtension
{
    public static Query WherePersonIs(this Query query, string personId)
    {
        return query.WhereEqualTo("person:id", personId);
    }

    public static TimeOnly ToTimeOnly(this int seconds)
    {
        return TimeOnly.FromTimeSpan(TimeSpan.FromSeconds(seconds));
    }

    public static int ToSeconds(this TimeOnly timeOnly)
    {
        return timeOnly.ToTimeSpan().Seconds;
    }

    public static DateOnly ToDateOnly(this Timestamp timestamp)
    {
        return DateOnly.FromDateTime(timestamp.ToDateTime());
    }

    public static Timestamp ToTimespan(this DateOnly dateOnly)
    {
        return Timestamp.FromDateTime(
            dateOnly.ToDateTime(
                TimeOnly.MinValue,
                DateTimeKind.Utc));
    }
}