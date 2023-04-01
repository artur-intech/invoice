namespace Intech.Invoice;

class Timezone
{
    public static TimeZoneInfo Default()
    {
        var envTz = Environment.GetEnvironmentVariable("TIME_ZONE");
        return envTz is not null ? TimeZoneInfo.FindSystemTimeZoneById(envTz) : TimeZoneInfo.Local;
    }

    public static TimeZoneInfo Fake()
    {
        var oneHour = new TimeSpan(1, 0, 0);
        return TimeZoneInfo.CreateCustomTimeZone(id: "fake", baseUtcOffset: oneHour, displayName: null, standardDisplayName: null, daylightDisplayName: null, adjustmentRules: null, disableDaylightSavingTime: true);
    }
}
