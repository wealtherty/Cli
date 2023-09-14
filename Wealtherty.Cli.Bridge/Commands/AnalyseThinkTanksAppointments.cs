using System.Globalization;
using CommandLine;
using CsvHelper;
using Microsoft.Extensions.DependencyInjection;
using Wealtherty.Cli.Bridge.Model.Csv;
using Wealtherty.Cli.Core;

namespace Wealtherty.Cli.Bridge.Commands;

[Verb("br:analyse-thinktanks-appointments")]
public class AnalyseThinkTanksAppointments : Command
{
    protected override async Task ExecuteImplAsync(IServiceProvider serviceProvider)
    {
        var inputReader = serviceProvider.GetRequiredService<InputReader>();
        
        var allAppointments = inputReader.ReadCsv<ThinkTankAppointment>("all_appointments.csv");

        var years = new[] { 2000, 2005, 2010, 2015 };

        var dates = years.Select(year => new KeyValuePair<string, DateRange>($"{year}_to_{year + 5}",
                new DateRange
                {
                    From = new DateTime(year, 1, 1),
                    To = new DateTime(year, 1, 1).AddYears(5).AddMilliseconds(-1)
                }))
            .ToDictionary(x => x.Key, x => x.Value);

        foreach (var date in dates)
        {
            var appointmentsForDateRange = allAppointments
                .Where(x => x.ThinkTankFoundedOn <= date.Value.To)
                .Where(x => x.CompanyDateOfCreation.HasValue && x.CompanyDateOfCreation <= date.Value.To)
                .Where(x => !x.CompanyDateOfCessation.HasValue || (x.CompanyDateOfCessation.HasValue && x.CompanyDateOfCessation >= date.Value.From))
                .Where(x => x.OfficerAppointedOn.HasValue && x.OfficerAppointedOn <= date.Value.To)
                .Where(x => !x.OfficerResignedOn.HasValue || (x.OfficerResignedOn.HasValue && x.OfficerResignedOn >= date.Value.From))
                .ToArray();
            
            await WriteToCsvFileAsync(appointmentsForDateRange, $"appointments_for_{date.Key}.csv");

            var sicCodesForDateRange = appointmentsForDateRange.GroupBy(x => new
                {
                    x.ThinkTankPoliticalWing,
                    x.CompanySicCode,
                    x.CompanySicCodeCategory,
                    x.CompanySicCodeDescription
                })
                .Select(x => new PoliticalSicCodeDescription
                {
                    PoliticalWing = x.Key.ThinkTankPoliticalWing,
                    SicCode = x.Key.CompanySicCode,
                    SicCodeCategory = x.Key.CompanySicCodeCategory,
                    SicCodeDescription = x.Key.CompanySicCodeDescription,
                    Count = x.ToList().Count
                })
                .OrderBy(x => x.PoliticalWing)
                .ThenByDescending(x => x.Count)
                .ToArray();
            
            await WriteToCsvFileAsync(sicCodesForDateRange, $"sic_codes_for_{date.Key}.csv");

            var sicCodeCategoriesForDateRange = appointmentsForDateRange.GroupBy(x => new
                {
                    x.ThinkTankPoliticalWing,
                    x.CompanySicCodeCategory
                })
                .Select(x => new PoliticalSicCodeCategory
                {
                    PoliticalWing = x.Key.ThinkTankPoliticalWing,
                    SicCodeCategory = x.Key.CompanySicCodeCategory,
                    Count = x.ToList().Count
                })
                .OrderBy(x => x.PoliticalWing)
                .ThenByDescending(x => x.Count)
                .ToArray();
            
            await WriteToCsvFileAsync(sicCodeCategoriesForDateRange, $"sic_code_categories_for_{date.Key}.csv");
        }
    }

    private static async Task WriteToCsvFileAsync<T>(IEnumerable<T> rows, string path)
    {
        await using var writer = new StreamWriter(path);
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        await csv.WriteRecordsAsync(rows);
    }
}