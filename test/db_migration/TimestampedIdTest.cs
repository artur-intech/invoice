using Intech.Invoice.DbMigration;
using NUnit.Framework;

namespace Intech.Invoice.Test;

class TimestampedIdTest
{
    [Test]
    public void RepresentsAsString()
    {
        var name = "test";
        var oneHour = new TimeSpan(1, 0, 0);
        var oneHourAheadTimeZone = TimeZoneInfo.CreateCustomTimeZone("any", oneHour, null, null, null, null, true);

        var timestampedId = new TimestampedId(name, new Clock.Fake("1970-01-01 08:00", oneHourAheadTimeZone));

        Assert.AreEqual($"19700101090000_{name}", $"{timestampedId}");
    }
}
