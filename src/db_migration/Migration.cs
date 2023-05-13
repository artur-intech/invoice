namespace Intech.Invoice.DbMigration;

interface Migration
{
    class Fake : Migration
    {
        bool pending;

        public Fake(bool pending = true)
        {
            this.pending = pending;
        }

        public void Apply()
        {
            pending = false;
        }

        public string Id()
        {
            return "1234";
        }

        public bool Pending()
        {
            return pending;
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
