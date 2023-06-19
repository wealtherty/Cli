using AutoMapper;
using Wealtherty.Cli.CompaniesHouse.Model;

namespace Wealtherty.Cli.CompaniesHouse;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<global::CompaniesHouse.Response.CompanyProfile.CompanyProfile, Company>();
    }
}