using AutoMapper;
using Humanizer;
using Wealtherty.Cli.CompaniesHouse.Model;

namespace Wealtherty.Cli.CompaniesHouse.Mapping.Resolvers;

public class OfficerNameResolver : IValueResolver<global::CompaniesHouse.Response.Officers.Officer, Officer, string>
{
    public string Resolve(global::CompaniesHouse.Response.Officers.Officer source, Officer destination, string destMember, ResolutionContext context)
    {
        var formattedName = source.Name.ToLower().Transform(To.TitleCase);
        var parts = formattedName.Split(", ");
        Array.Reverse(parts);
        var reversed = string.Join(" ", parts);
        return reversed;
    }
}