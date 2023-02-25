using NUnit.Framework;

namespace Intech.Invoice.Test;

public class TimestampedNumberTest
{
    [Test]
    public void RepresentsItselfAsString()
    {
        var oneHourTimeSpan = new TimeSpan(1, 0, 0);
        var fakeTimeZone = TimeZoneInfo.CreateCustomTimeZone("any", oneHourTimeSpan, null, null, null, null, true);
        var fakeClock = new Clock.Fake("2023-01-02 08:00:01", fakeTimeZone);

        var timestampedNumber = new TimestampedNumber(fakeClock);

        Assert.AreEqual("20230102090001", $"{timestampedNumber}");
    }
}