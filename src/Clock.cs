namespace Intech.Invoice;

interface Clock
{
    class Fake : Clock
    {
        private readonly string currentTime;
        private readonly TimeZoneInfo timeZone;

        public Fake(string currentTime, TimeZoneInfo timeZone)
        {
            this.currentTime = currentTime;
            this.timeZone = timeZone;
        }

        public DateTime Now()
        {
            return DateTime.Parse(currentTime);
        }

        public DateOnly Today()
        {
            return DateOnly.FromDateTime(Now());
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

    DateTime Now();
    DateOnly Today();
    DateTime NowInAppTimeZone();
    DateOnly TodayInAppTimeZone();
}