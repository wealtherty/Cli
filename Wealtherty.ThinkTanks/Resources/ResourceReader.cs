using System.Globalization;
using System.Reflection;
using CsvHelper;
using Wealtherty.ThinkTanks.Csv.Model;

namespace Wealtherty.ThinkTanks.Resources;

public class ResourceReader
{
    private readonly Assembly _assembly;

    public ResourceReader()
    {
        _assembly = GetType().Assembly;
    }
    
    public ThinkTank[] GetThinkTanks()
    {
        return GetCsvResource<ThinkTank>("ThinkTanks");
    }
    
    public Company[] GetCompanies()
    {
        return GetCsvResource<Company>("Companies");
    }

    public Appointment[] GetAppointments()
    {
        return GetCsvResource<Appointment>("Appointments");
    }


    private T[] GetCsvResource<T>(string name)
    {
        var resourceName = $"{GetType().Namespace}.{name}.csv";
        
        using var stream = _assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records = csv.GetRecords<T>();

        return records.ToArray();
    }
}