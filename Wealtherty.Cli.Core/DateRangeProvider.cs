using Wealtherty.Cli.Core.Model;

namespace Wealtherty.Cli.Core;

public class DateRangeProvider
{
    public DateRange[] GetDateRangesForYears(int[] years)
    {
        var dateRanges = new List<DateRange>();

        for (var i = 0; i < years.Length - 1; i++)
        {
            var fromYear = years[i];
            var toYear = years[i + 1];

            dateRanges.Add(new DateRange
            {
                Description = $"{fromYear} to {toYear}",
                From = new DateTime(fromYear, 1, 1),
                To = new DateTime(toYear, 1, 1).AddMilliseconds(-1)
            });
        }
        
        return dateRanges.ToArray();
    }
}