namespace Wealtherty.Cli.CharityCommission.Mapping;

public class Profile : AutoMapper.Profile
{
    public Profile()
    {
        CreateMap<Api.Model.Chairty, Graph.Model.Chairty>();
    }
}