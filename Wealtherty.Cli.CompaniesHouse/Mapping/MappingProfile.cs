using AutoMapper;
using CompaniesHouse.Response.CompanyProfile;
using Wealtherty.Cli.CompaniesHouse.Mapping.Resolvers;
using Wealtherty.Cli.CompaniesHouse.Model;

namespace Wealtherty.Cli.CompaniesHouse.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CompanyProfile, Company>()
            .ForMember(x => x.Name, opt => opt.MapFrom<CompanyNameResolver>())
            .ForMember(x => x.Number, opt => opt.MapFrom(x => x.CompanyNumber))
            .ForMember(x => x.Status, opt => opt.MapFrom(x => x.CompanyStatus.ToString()))
            .ForMember(x => x.Status, opt => opt.MapFrom(x => x.CompanyStatus.ToString()))
            .ForMember(x => x.CreatedOn, opt => opt.MapFrom<ComnpanyCreatedOnResolver>())
            .ForMember(x => x.StoppedTradingOn, opt => opt.MapFrom<StoppedTradingOnResolver>());
        
        CreateMap<global::CompaniesHouse.Response.Officers.Officer, Officer>()
            .ForMember(x => x.Name, opt => opt.MapFrom<OfficerNameResolver>())
            .ForMember(x => x.OfficerId, opt => opt.MapFrom(x => x.Links.Officer.OfficerId))
            .ForMember(x => x.YearOfBirth, opt => opt.MapFrom(x => x.DateOfBirth.Year));
    }
}