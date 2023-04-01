using System.Collections;
using System.Collections.Specialized;
using System.Text;

namespace Intech.Invoice;

sealed class ConsoleMedia
{
    // The only type that preserves insertion order, which is a requirement here.
    readonly OrderedDictionary builder;

    public ConsoleMedia()
    {
        builder = new OrderedDictionary();
    }

    public ConsoleMedia(OrderedDictionary builder)
    {
        this.builder = builder;
    }

    public ConsoleMedia With(string name, object value)
    {
        builder.Add(name, value);
        return new ConsoleMedia(builder);
    }

    public string Text()
    {
        var stringBuilder = new StringBuilder();

        foreach (DictionaryEntry keyValueEntry in builder)
        {
            stringBuilder.AppendLine($"{keyValueEntry.Key}: {keyValueEntry.Value}");
        }

        return stringBuilder.ToString().TrimEnd();
    }
}
