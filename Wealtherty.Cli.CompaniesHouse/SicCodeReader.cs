using System.Globalization;
using System.Reflection;
using CsvHelper;
using Serilog;
using Wealtherty.Cli.CompaniesHouse.Graph.Model;

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
            var sicCode = _sicCodes.SingleOrDefault(x => x.Code.Equals(code));

            if (sicCode != null)
            {
                return sicCode;
            }
            
            Log.Warning("SIC Code not found - Code: {Code}", code);
            return new SicCode
            {
                Code = code,
                Description = "UNKNOWN"
            };
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