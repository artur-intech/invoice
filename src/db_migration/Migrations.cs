namespace Intech.Invoice.DbMigration;

interface Migrations
{
    void Init();
    void CreateEmpty(Id id);
}
