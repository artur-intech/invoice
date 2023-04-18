namespace Intech.Invoice;

interface EmailTemplate
{
    class Fake : EmailTemplate
    {
        readonly string text;

        public Fake(string text)
        {
            this.text = text;
        }

        public override string ToString()
        {
            return text;
        }
    }

    string ToString();
}
