namespace Intech.Invoice;

sealed class SystemClock : Clock
{
    readonly TimeZoneInfo timeZone;

    public SystemClock(TimeZoneInfo timeZone)
    {
        this.timeZone = timeZone;
    }

    public DateTime Now()
    {
        return DateTime.Now;
    }

    public DateOnly Today()
    {
        return DateOnly.FromDateTime(DateTime.Now);
    }

    public DateTime NowInAppTimeZone()
    {
        return TimeZoneInfo.ConvertTime(Now(), timeZone);
    }
    public DateOnly TodayInAppTimeZone()
    {
        return DateOnly.FromDateTime(NowInAppTimeZone());
    }
}