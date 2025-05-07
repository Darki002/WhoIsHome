namespace WhoIsHome.Host.BackgroundTasks.DatabaseCleanUpTask;

public sealed record DbCleanUpTaskOptions
{
    /// <summary>
    /// Day of week on which to run (e.g. "Saturday", "Sunday", etc.)
    /// </summary>
    public DayOfWeek DayOfWeek { get; set; } = DayOfWeek.Sunday;

    /// <summary>
    /// Time of day (HH:mm:ss) on that day to run the job
    /// </summary>
    public TimeSpan Time { get; set; } = TimeSpan.FromHours(2);
    
    /// <summary>
    /// How many days of data to keep (older records will be deleted)
    /// </summary>
    public int DaysToKeep { get; set; } = 90;
}