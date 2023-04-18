using NUnit.Framework;

namespace Intech.Invoice.Test;

class InterpolatedEmailBodyTemplateTest : Base
{
    [Test]
    public void Interpolates()
    {
        var text = "{0}, here is your invoice. Total: {1}, due date: {2}. Yours, {3}";
        var dueDate = ValidDate();
        var total = 100;
        var clientName = "john";
        var supplierName = "bruce";

        var template = new InterpolatedEmailTemplate(new EmailTemplate.Fake(text), dueDate, total, clientName, supplierName);
        var actual = $"{template}";

        Assert.AreEqual($"{clientName}, here is your invoice. Total: {total}, due date: {dueDate}. Yours, {supplierName}", actual);
    }
}
