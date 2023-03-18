namespace Intech.Invoice;

interface Clock
{
    class Fake : Clock
    {
        private readonly string iso8601Now;
        private readonly TimeZoneInfo timeZone;

        public Fake() : this("1970-01-01 07:00:00", Timezone.Fake()) { }

        public Fake(string iso8601Time) : this(iso8601Time, Timezone.Fake()) { }

        public Fake(string iso8601Time, TimeZoneInfo timeZone)
        {
            iso8601Now = iso8601Time;
            this.timeZone = timeZone;
        }

        public DateTime Now()
        {
            return DateTime.Parse(iso8601Now);
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