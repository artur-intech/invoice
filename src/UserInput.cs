namespace Intech.Invoice;

interface UserInput
{
    class Fake : UserInput
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
