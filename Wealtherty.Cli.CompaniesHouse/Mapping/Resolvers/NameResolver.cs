using AutoMapper;
using Wealtherty.Cli.CompaniesHouse.Model;

namespace Wealtherty.Cli.CompaniesHouse.Mapping.Resolvers;

public class NameResolver : IValueResolver<global::CompaniesHouse.Response.Officers.Officer, Officer, string>
{
    public string Resolve(global::CompaniesHouse.Response.Officers.Officer source, Officer destination, string destMember, ResolutionContext context)
    {
        var parts = source.Name.Split(", ");
        Array.Reverse(parts);
        var reversed = string.Join(" ", parts);
        return reversed;
    }
}