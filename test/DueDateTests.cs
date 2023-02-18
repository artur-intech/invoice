using NUnit.Framework;

namespace Intech.Invoice.Test;

class DueDateTests
{
    [Test]
    public void CreatesStandard()
    {
        const int days = 10;
        var startDate = new DateOnly(1970, 1, 1);

        var standardDueDate = DueDate.Standard(startDate);

        Assert.AreEqual(startDate.AddDays(days), standardDueDate.Date());
    }
}