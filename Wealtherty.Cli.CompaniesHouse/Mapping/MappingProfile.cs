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
            .ForMember(x => x.Name, opt => opt.MapFrom(x => x.CompanyName))
            .ForMember(x => x.Number, opt => opt.MapFrom(x => x.CompanyNumber))
            .ForMember(x => x.Status, opt => opt.MapFrom(x => x.CompanyStatus.ToString()))
            .ForMember(x => x.Status, opt => opt.MapFrom(x => x.CompanyStatus.ToString()))
            .ForMember(x => x.CreatedOn, opt => opt.MapFrom(x => x.DateOfCreation))
            .ForMember(x => x.StoppedTradingOn, opt => opt.MapFrom(x => x.DateOfCessation));
        
        CreateMap<global::CompaniesHouse.Response.Officers.Officer, Officer>()
            .ForMember(x => x.Name, opt => opt.MapFrom<NameResolver>())
            .ForMember(x => x.OfficerId, opt => opt.MapFrom(x => x.Links.Officer.OfficerId))
            .ForMember(x => x.YearOfBirth, opt => opt.MapFrom(x => x.DateOfBirth.Year));
    }
}