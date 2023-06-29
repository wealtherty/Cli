namespace Wealtherty.Cli.Ukri.Mapping;

public class Profile : AutoMapper.Profile
{
    public Profile()
    {
        CreateMap<Api.Model.Project, Graph.Model.Project>()
            .ForMember(x => x.ProjectId, opts => opts.MapFrom(x => x.Id));
        CreateMap<Api.Model.Organisation, Graph.Model.Organisation>()
            .ForMember(x => x.OrganisationId, opts => opts.MapFrom(x => x.Id));
        CreateMap<Api.Model.Person, Graph.Model.Person>()
            .ForMember(x => x.PersonId, opts => opts.MapFrom(x => x.Id));
        CreateMap<Api.Model.Fund, Graph.Model.Fund>()
            .ForMember(x => x.FundId, opts => opts.MapFrom(x => x.Id))
            .ForMember(x => x.Amount, opts => opts.MapFrom(x => x.Value.Amount));
    }
}