namespace Intech.Invoice.DbMigration;

interface Migration
{
    class Fake : Migration
    {
        public void Apply()
        {
        }

        public string Id()
        {
            return "1234";
        }

        public bool Pending()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Id();
        }
    }

    void Apply();
    bool Pending();
    // Consider hiding. Currently only used by tests.
    string Id();
    string ToString();
}
