using System.Globalization;
using CsvHelper;

namespace Wealtherty.Cli.Core;

public class InputReader
{
    public T[] ReadCsv<T>(string name)
    {
        using var reader = new StreamReader($"{name}");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records = csv.GetRecords<T>();
        return records.ToArray();
    }
}