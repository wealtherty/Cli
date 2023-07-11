using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Wealtherty.Cli.CompaniesHouse.Model;
using Wealtherty.Cli.Core;

namespace Wealtherty.Cli.CompaniesHouse.Commands;

[Verb("ch:thinktanks-get")]
public class GetThinkTanks : Command
{
    private static class Companies
    {
        public const string AdamSmithInstitute = null;
        public const string BowGroup = null;
        public const string CentreForPolicyStudies = "01174651";
        public const string CentreForSocialJustice = "05137036";
        public const string InstituteOfEconomicAffairs = null;
        public const string PolicyExchange = "04297905";
        public const string ReformResearchTrust = "05064109";
        public const string LegatumInstitute = "FC028686";
        public const string LegatumInstituteLimited = "14543238";
    }

    private static IEnumerable<ThinkTank> ThinkTanks
    {
        get
        {
            yield return new ThinkTank
            {
                Name = "Adam Smith Institude",
                CompanyNumber = Companies.AdamSmithInstitute,
                Wing = PoliticalWing.Right
            };
            yield return new ThinkTank
            {
                Name = "Bow Group",
                CompanyNumber = Companies.BowGroup,
                Wing = PoliticalWing.Right
            };
            yield return new ThinkTank
            {
                Name = "Center for Policy Studies",
                CompanyNumber = Companies.CentreForPolicyStudies,
                Wing = PoliticalWing.Right
            };
            yield return new ThinkTank
            {
                Name = "Center for Social Justice",
                CompanyNumber = Companies.CentreForSocialJustice,
                Wing = PoliticalWing.Right
            };
            yield return new ThinkTank
            {
                Name = "Institute for Economic Affairs",
                CompanyNumber = Companies.InstituteOfEconomicAffairs,
                Wing = PoliticalWing.Right
            };
            yield return new ThinkTank
            {
                Name = "Policy Exchange",
                CompanyNumber = Companies.PolicyExchange,
                Wing = PoliticalWing.Right
            };
            yield return new ThinkTank
            {
                Name = "Reform Research Trust",
                CompanyNumber = Companies.ReformResearchTrust,
                Wing = PoliticalWing.Right
            };
            yield return new ThinkTank
            {
                Name = "Legatum Institute",
                CompanyNumber = Companies.LegatumInstitute,
                Wing = PoliticalWing.Right
            };
            yield return new ThinkTank
            {
                Name = "Legatum Institute",
                CompanyNumber = Companies.LegatumInstituteLimited,
                Wing = PoliticalWing.Right
            };
        }
    }


    protected override async Task ExecuteImplAsync(IServiceProvider serviceProvider)
    {
        var facade = serviceProvider.GetService<Facade>();
        
        var cancellationToken = new CancellationToken();
        
        foreach (var thinkTank in ThinkTanks)
        {
            await facade.ModelThinkTankAsync(thinkTank.Name, thinkTank.Wing, thinkTank.CompanyNumber, cancellationToken);
        }
    }
    
    private class ThinkTank
    {
        public string Name { get; set; }
        
        public PoliticalWing Wing { get; set; }
        
        public string CompanyNumber { get; set; }
    }
}