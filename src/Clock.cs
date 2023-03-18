namespace Intech.Invoice;

interface Clock
{
    class Fake : Clock
    {
        private readonly string currentTime;
        private readonly TimeZoneInfo timeZone;

        public Fake() : this("1970-01-01 07:00:00", Timezone.Fake()) { }

        public Fake(string currentTime) : this(currentTime, Timezone.Fake()) { }

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