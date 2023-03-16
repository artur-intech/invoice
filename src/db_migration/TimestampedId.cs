namespace Intech.Invoice.DbMigration;

sealed class TimestampedId : Id
{
    readonly string name;
    readonly Clock clock;

    public TimestampedId(string name, Clock clock)
    {
        this.name = name;
        this.clock = clock;
    }

    public override string ToString()
    {
        return $"{Timestamp()}_{name}";
    }

    string Timestamp()
    {
        return clock.NowInAppTimeZone().ToString("yyyyMMddHHmmss");
    }
}