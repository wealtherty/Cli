using CompaniesHouse.Response.Officers;
using Neo4j.Driver;
using Serilog;
using Wealtherty.Cli.CompaniesHouse.Graph.Model;
using Wealtherty.Cli.Core;
using Wealtherty.Cli.Core.GraphDb;
using Officer = Wealtherty.Cli.CompaniesHouse.Graph.Model.Officer;

namespace Wealtherty.Cli.CompaniesHouse;

public class Facade
{
    private readonly IAsyncSession _session;
    private readonly Client _client;
    private readonly SicCodeReader _sicCodeReader;

    private static readonly OfficerRole[] RolesToIgnore = {
        OfficerRole.CorporateNomineeDirector,
        OfficerRole.CorporateNomineeSecretary,
        OfficerRole.CorporateSecretary,
        OfficerRole.NomineeDirector, 
        OfficerRole.NomineeSecretary
    };

    private static readonly string[] CompaniesToIgnore = Array.Empty<string>();

    private static readonly string[] OfficersToIgnore =
    {
        "SBjtBss_I4XEupbfAUXoeAkMcIk",
        "8d_bnTiwfxh8JIr3YfuwkmkWkCg"
    };

    public Facade(Client client, SicCodeReader sicCodeReader, IAsyncSession session)
    {
        _client = client;
        _sicCodeReader = sicCodeReader;
        _session = session;
    }
    
    public async Task<Company> ModelCompanyAsync(string companyNumber, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(companyNumber)) return null;
        
        if (CompaniesToIgnore.Contains(companyNumber, StringComparer.OrdinalIgnoreCase))
        {
            Log.Information("Ignoring Company: {Number}", companyNumber);
            return null;
        }

        Company company = null;
        
        var officers = await _client.GetOfficersAsync(companyNumber, cancellationToken);

        foreach (var officer in officers)
        {
            if (OfficersToIgnore.Contains(officer.Links.Officer.OfficerId, StringComparer.OrdinalIgnoreCase) ||  RolesToIgnore.Contains(officer.OfficerRole))
            {
                Log.Information("Ignoring Officer: {@Officer}", new { Id = officer.Links.Officer.OfficerId, officer.Name, Role = officer.OfficerRole});
                continue;
            }
            
            var officerNode = new Officer(officer);

            var appointments = await _client.GetAppointmentsAsync(officer.Links.Officer.OfficerId, cancellationToken);
            
            foreach (var appointment in appointments.Where(x => !RolesToIgnore.Contains(x.OfficerRole)))
            {
                if (CompaniesToIgnore.Contains(appointment.Appointed.CompanyNumber, StringComparer.OrdinalIgnoreCase))
                {
                    Log.Information("Ignoring Appointment: {@Appointment}", new { OfficeId = officer.Links.Officer.OfficerId, officer.Name, appointment.Appointed.CompanyNumber, appointment.Appointed.CompanyName});
                    continue;
                }
                
                var getAppointmentCompanyResponse = await _client.GetCompanyProfileAsync(appointment.Appointed.CompanyNumber, cancellationToken);
                var companyProfile = getAppointmentCompanyResponse.Data;
                
                var companyProfileNode = new Company(companyProfile);
                companyProfileNode.AddRelations(companyProfile.SicCodes.OrEmpty().Select(code => new Relationship<Company, SicCode>(companyProfileNode, _sicCodeReader.Read(code), "NATURE_OF_BUSINESS")));
                officerNode.AddRelation(new Appointment(officerNode, companyProfileNode, appointment));

                if (companyProfileNode.Number.Equals(companyNumber))
                {
                    company = companyProfileNode;
                }
            }

            await _session.ExecuteCommandsAsync(officerNode);
        }

        return company;
    }
    
    public async Task<Company> GetCompanyAsync(string companyNumber)
    {
        return await _session.GetNodeAsync<Company>("MATCH (c:Company) WHERE c.Number = $Number RETURN c",
            new Dictionary<string, object>
            {
                { "Number", companyNumber }
            });
    }
}