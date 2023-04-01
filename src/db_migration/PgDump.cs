using System.Diagnostics;

namespace Intech.Invoice.DbMigration;

class PgDump : DumpUtil
{
    readonly string connUri;

    public PgDump(string connUri)
    {
        this.connUri = connUri;
    }

    public void DumpToFile(string path, IEnumerable<string> excludedDataTables)
    {
        var process = new Process();
        process.StartInfo.FileName = "pg_dump";
        process.StartInfo.Arguments = $"-d {connUri} -f {path} {ExcludedDataTablesArguments(excludedDataTables)}";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.Start();

        var stdout = process.StandardOutput.ReadToEnd();
        Console.WriteLine(stdout);
        process.WaitForExit();
    }

    string ExcludedDataTablesArguments(IEnumerable<string> excludedDataTables)
    {
        return string.Join(' ', excludedDataTables.Select(table => $"--exclude-table-data={table}"));
    }
}
