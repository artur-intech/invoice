using NUnit.Framework;

namespace Intech.Invoice.Test;

class VatRateTest
{
    string? originalEnvVatRate;

    [SetUp]
    protected void SaveOriginalEnvValue()
    {
        originalEnvVatRate = Environment.GetEnvironmentVariable("STANDARD_VAT_RATE");
    }

    [Test]
    public void CreatesStandardVatRateFromEnv()
    {
        var envRate = "25";
        Assert.AreNotEqual(envRate, originalEnvVatRate);
        Environment.SetEnvironmentVariable("STANDARD_VAT_RATE", envRate);

        var standard = VatRate.Standard();

        Assert.AreEqual(int.Parse(envRate), standard.IntValue());
    }

    [TearDown]
    protected void RestoreOriginalEnvValue()
    {
        Environment.SetEnvironmentVariable("STANDARD_VAT_RATE", originalEnvVatRate);
    }
}