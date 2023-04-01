namespace Intech.Invoice;

sealed class TimestampedNumber : Number
{
    readonly Clock clock;

    public TimestampedNumber(Clock clock)
    {
        this.clock = clock;
    }

    public override string ToString()
    {
        return clock.NowInAppTimeZone().ToString("yyyyMMddHHmmss");
    }
}
