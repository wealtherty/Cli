using AutoMapper;
using CompaniesHouse.Response.CompanyProfile;
using Wealtherty.Cli.CompaniesHouse.Model;
using Wealtherty.Cli.Core;

namespace Wealtherty.Cli.CompaniesHouse.Mapping.Resolvers;

public class ComnpanyCreatedOnResolver : IValueResolver<CompanyProfile, Company, string>
{
    public string Resolve(CompanyProfile source, Company destination, string destMember, ResolutionContext context) => source.DateOfCreation.ToNeo4jDate();
}