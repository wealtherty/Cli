using AutoMapper;
using Wealtherty.Cli.CompaniesHouse.Model;

namespace Wealtherty.Cli.CompaniesHouse;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<global::CompaniesHouse.Response.CompanyProfile.CompanyProfile, Company>()
            .ForMember(x => x.Number, opts => opts.MapFrom(cp => cp.CompanyNumber))
            .ForMember(x => x.Name, opts => opts.MapFrom(cp => cp.CompanyName));
    }
}