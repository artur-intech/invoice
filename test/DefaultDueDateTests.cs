using NUnit.Framework;

namespace Intech.Invoice.Test;

class DefaultDueDateTests
{
    [Test]
    public void CalculatesDate()
    {
        var today = new DateOnly(1970, 1, 1);
        var days = 1;

        var defaultDueDate = new DefaultDueDate(days: days, startDate: today);

        Assert.AreEqual(today.AddDays(days), defaultDueDate.Date());
    }
}