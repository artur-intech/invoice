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

sealed class InterpolatedEmailTemplate : EmailTemplate
{
    readonly EmailTemplate origin;
    readonly DateOnly dueDate;
    readonly long total;
    readonly string clientName;

    public InterpolatedEmailTemplate(EmailTemplate origin, DateOnly dueDate, long total, string clientName)
    {
        this.origin = origin;
        this.dueDate = dueDate;
        this.total = total;
        this.clientName = clientName;
    }

    public override string ToString()
    {
        return string.Format(origin.ToString(), clientName, total, dueDate);
    }
}
