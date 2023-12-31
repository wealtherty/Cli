﻿using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Wealtherty.Cli.Core;
using Wealtherty.ThinkTanks.Model.Csv;
using Wealtherty.ThinkTanks.Resources;

namespace Wealtherty.ThinkTanks.Commands;

[Verb("tt:analyse-appointments")]
public class AnalyseAppointments : Command
{
    protected override async Task ExecuteImplAsync(IServiceProvider serviceProvider)
    {
        var resourceReader = serviceProvider.GetRequiredService<ResourceReader>();
        var outputWriter = serviceProvider.GetRequiredService<OutputWriter>();
        var dateRangesProvider = serviceProvider.GetRequiredService<DateRangeProvider>();
        
        var appointments = resourceReader.GetAppointments();

        var years = new[] { 2000, 2005, 2010, 2015, 2020 };

        var dateRanges = dateRangesProvider.GetDateRangesForYears(years);

        foreach (var dateRange in dateRanges)
        {
            var appointmentsForDateRange = appointments
                .Where(x => x.ThinkTankFoundedOn <= dateRange.To)
                .Where(x => x.CompanyDateOfCreation.HasValue && x.CompanyDateOfCreation <= dateRange.To)
                .Where(x => !x.CompanyDateOfCessation.HasValue || (x.CompanyDateOfCessation.HasValue && x.CompanyDateOfCessation >= dateRange.From))
                .Where(x => x.OfficerAppointedOn.HasValue && x.OfficerAppointedOn <= dateRange.To)
                .Where(x => !x.OfficerResignedOn.HasValue || (x.OfficerResignedOn.HasValue && x.OfficerResignedOn >= dateRange.From))
                .ToArray();
            
            await outputWriter.WriteToCsvFileAsync(appointmentsForDateRange, $"appointments for {dateRange.Description}.csv");

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
            
            await outputWriter.WriteToCsvFileAsync(sicCodesForDateRange, $"sic codes for {dateRange.Description}.csv");

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
            
            await outputWriter.WriteToCsvFileAsync(sicCodeCategoriesForDateRange, $"sic code categories for {dateRange.Description}.csv");
        }
    }
}