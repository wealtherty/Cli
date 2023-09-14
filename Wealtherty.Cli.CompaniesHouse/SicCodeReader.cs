using System.Globalization;
using System.Reflection;
using CsvHelper;
using Wealtherty.Cli.CompaniesHouse.Model.Csv;

namespace Wealtherty.Cli.CompaniesHouse
{
    public class SicCodeReader
    {
        private readonly SicCode[] _sicCodes;

        public SicCodeReader()
        {
            _sicCodes = ReadSicCodes();
        }

        public SicCode Read(string code)
        {
            return _sicCodes.SingleOrDefault(x => x.Code.Equals(code));
        }
        
        private static SicCode[] ReadSicCodes()
        {
            const string resourceName = "Wealtherty.Cli.CompaniesHouse.Resources.SicCodes.csv";
            
            var assembly = Assembly.GetExecutingAssembly();

            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<SicCode>();

            return records.ToArray();
        }
    }
}