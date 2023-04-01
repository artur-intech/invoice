namespace Intech.Invoice.DbMigration;

interface Id
{
    class Fake : Id
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
}
