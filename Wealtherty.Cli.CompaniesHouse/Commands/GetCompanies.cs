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
        private const string InstituteOfEconomicAffairs = null;
        private const string PolicyExchange = "04297905";
        private const string ReformResearchTrust = "05064109";
        private const string LegatumInstitute = "FC028686";
        private const string LegatumInstituteLimited = "14543238";
        
        public static IEnumerable<string> All()
        {
            return new[]
            {
                AdamSmithInstitute,
                BowGroup,
                CentreForPolicyStudies,
                CentreForSocialJustice,
                InstituteOfEconomicAffairs,
                PolicyExchange,
                ReformResearchTrust,
                LegatumInstitute,
                LegatumInstituteLimited
            };
        }
    }
    
    protected override async Task ExecuteImplAsync(IServiceProvider serviceProvider)
    {
        var facade = serviceProvider.GetService<CompaniesHouseFacade>();
        
        var cancellationToken = new CancellationToken();
        
        foreach (var companyNumber in CompanyNumbers.All().Where(x => x != null))
        {
            await facade.ModelCompanyAsync(companyNumber, cancellationToken);
        }

    }
}