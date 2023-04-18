namespace Intech.Invoice;

sealed class InFileEmailTemplate : EmailTemplate
{
    readonly string path;

    public InFileEmailTemplate(string path)
    {
        this.path = path;
    }

    public override string ToString()
    {
        return Text();
    }

    string Text()
    {
        return File.ReadAllText(path);
    }
}
