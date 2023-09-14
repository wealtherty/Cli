using System.Globalization;
using CsvHelper;

namespace Wealtherty.Cli.Core;

public class OutputWriter
{
    private readonly string _dir;
    public OutputWriter()
    {
        _dir = $"output\\{DateTime.Now.ToString("yyyyMMdd_hhmmss")}\\";
    }
    
    public async Task WriteToCsvFileAsync<T>(IEnumerable<T> rows, string path)
    {
        var fullPath = Path.Join(_dir, path).Replace(' ', '_');

        Directory.CreateDirectory(_dir);
        
        await using var writer = new StreamWriter(fullPath);
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        await csv.WriteRecordsAsync(rows);
    }
}