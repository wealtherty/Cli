using System.Globalization;
using CsvHelper;

namespace Wealtherty.Cli.Core;

public class OutputWriter
{
    public async Task WriteToCsvFileAsync<T>(IEnumerable<T> rows, string path)
    {
        await using var writer = new StreamWriter(path.Replace(' ', '_'));
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        await csv.WriteRecordsAsync(rows);
    }

}