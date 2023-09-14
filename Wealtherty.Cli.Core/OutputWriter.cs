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
    
    public async Task WriteToCsvFileAsync<T>(IEnumerable<T> rows, string path, bool useOutputDirectory = true)
    {
        if (useOutputDirectory)
        {
            Directory.CreateDirectory(_dir);    
        }

        var fullPath = (useOutputDirectory ? Path.Join(_dir, path) : path).Replace(' ', '_');

        await using var writer = new StreamWriter(fullPath);
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        await csv.WriteRecordsAsync(rows);
    }
}