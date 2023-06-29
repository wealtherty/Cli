using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Wealtherty.Cli.Core;

namespace Wealtherty.Cli.CompaniesHouse.Commands;

[Verb("ch:companies-get")]
public class GetCompanies : Command
{
    private class CompanyNumbers
    {
        
        private const string AdamSmithInstitute = null;
        private const string BowGroup = null;
        private const string CentreForPolicyStudies = "01174651";
        private const string CentreForSocialJustice = "05137036";
        private const string InstitueOfEconomicAffairs = null;
        private const string PolicyExchange = "04297905";
        private const string Reform = null;
        private const string LegatumInstitute = null;
        
        public static IEnumerable<string> All()
        {
            return new[]
            {
                // AdamSmithInstitute,
                // BowGroup,
                // CentreForPolicyStudies,
                // CentreForSocialJustice,
                // InstitueOfEconomicAffairs,
                PolicyExchange,
                // Reform,
                // LegatumInstitute
            };
        }
    }
    
    protected override async Task ExecuteImplAsync(IServiceProvider serviceProvider)
    {
        var facade = serviceProvider.GetService<Facade>();
        
        var cancellationToken = new CancellationToken();
        
        foreach (var companyNumber in CompanyNumbers.All().Where(x => x != null))
        {
            await facade.ModelCompanyAsync(companyNumber, cancellationToken);
        }

    }
}