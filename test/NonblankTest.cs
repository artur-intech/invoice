using NUnit.Framework;

namespace Intech.Invoice.Test;

class NonblankTest
{
    [Test]
    [TestCase("")]
    [TestCase(" ")]
    public void ProhibitsBlank(string userInput)
    {
        var nonblank = new Nonblank(new UserInput.Fake(userInput));

        var exception = Assert.Throws(typeof(Exception), () =>
        {
            nonblank.ToString();
        });
        Assert.AreEqual("Value cannot be empty.", exception.Message);
    }
}
