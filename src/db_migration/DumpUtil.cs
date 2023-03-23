namespace Intech.Invoice.DbMigration;

interface DumpUtil
{
    void DumpToFile(string path, IEnumerable<string> excludedDataTables);
}