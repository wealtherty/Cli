using AutoMapper;
using CompaniesHouse.Response.CompanyProfile;
using Humanizer;
using Wealtherty.Cli.CompaniesHouse.Model;

namespace Wealtherty.Cli.CompaniesHouse.Mapping.Resolvers;

public class CompanyNameResolver : IValueResolver<CompanyProfile, Company, string>
{
    public string Resolve(CompanyProfile source, Company destination, string destMember, ResolutionContext context)
    {
        return source.CompanyName.ToLower().Transform(To.TitleCase);
    }
}