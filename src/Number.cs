namespace Intech.Invoice;

interface Number
{
    class Fake : Number
    {
        readonly string value;

        public Fake(string value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return value;
        }
    }

    string ToString();
}
