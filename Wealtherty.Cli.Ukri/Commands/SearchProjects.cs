using AutoMapper;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Neo4j.Driver;
using Wealtherty.Cli.Core;
using Wealtherty.Cli.Ukri.Api;
using Wealtherty.Cli.Ukri.Graph.Model;

namespace Wealtherty.Cli.Ukri.Commands
{
    [Verb("ukri:projects-search")]
    public class SearchProjects : Command
    {
        [Option('q', "query", Required = true)]
        public string Query { get; set; }
    
        protected override async Task ExecuteImplAsync(IServiceProvider serviceProvider)
        {
            var client = serviceProvider.GetService<Client>();
            var mapper = serviceProvider.GetService<IMapper>();
            var driver = serviceProvider.GetService<IDriver>();

            var projects = await client.SearchProjectsAsync(Query);
            
            await using var session = driver.AsyncSession();

            foreach (var project in projects)
            {
                var projectNode = mapper.Map<Project>(project);
            
                var organisationLinks = project.LinksWrapper.Links.Where(x => x.IsForOrganisation());
                foreach (var link in organisationLinks)
                {
                    var organisation = await client.GetOrganisationAsync(link.GetId());
                    var organisationNode = mapper.Map<Organisation>(organisation);
                    
                    projectNode.AddRelation(new NamedRelationship<Project, Organisation>(projectNode, organisationNode, link.Rel));
                }
                
                var personLinks = project.LinksWrapper.Links.Where(x => x.IsForPerson());
                foreach (var link in personLinks)
                {
                    var person = await client.GetPersonAsync(link.GetId());
                    var personNode = mapper.Map<Person>(person);
                    
                    projectNode.AddRelation(new NamedRelationship<Project, Person>(projectNode, personNode, link.Rel));
                }

                var fundLinks = project.LinksWrapper.Links.Where(x => x.IsForFund());
                foreach (var link in fundLinks)
                {
                    var fund = await client.GetFundAsync(link.GetId());
                    var fundNode = mapper.Map<Fund>(fund);
                    
                    projectNode.AddRelation(new NamedRelationship<Project, Fund>(projectNode, fundNode, link.Rel));
                }
                
                await session.ExecuteCommandsAsync(projectNode);
            }
        }
    }
}