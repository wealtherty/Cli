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
    
    public async Task<Company> CreateCompanyAsync(string companyNumber, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(companyNumber)) return null;

        var safeCompanyNumber = companyNumber.PadLeft(8, '0');
        
        if (CompaniesToIgnore.Contains(safeCompanyNumber, StringComparer.OrdinalIgnoreCase))
        {
            Log.Information("Ignoring Company: {Number}", safeCompanyNumber);
            return null;
        }
        
        var getAppointmentCompanyResponse = await _client.GetCompanyProfileAsync(safeCompanyNumber, cancellationToken);
        var companyProfile = getAppointmentCompanyResponse.Data;
                
        var companyProfileNode = new Company(companyProfile);

        await _session.ExecuteCommandsAsync(companyProfileNode);
        
        return companyProfileNode;
    }
    
    
    public async Task CreateOfficersAsync(string companyNumber, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(companyNumber)) return;

        var safeCompanyNumber = companyNumber.PadLeft(8, '0');
        
        if (CompaniesToIgnore.Contains(safeCompanyNumber, StringComparer.OrdinalIgnoreCase))
        {
            Log.Information("Ignoring Company: {Number}", safeCompanyNumber);
            return;
        }
        
        var getAppointmentCompanyResponse = await _client.GetCompanyProfileAsync(safeCompanyNumber, cancellationToken);
        var companyProfile = getAppointmentCompanyResponse.Data;

        var companyProfileNode = new Company(companyProfile);
        companyProfileNode.AddRelations(companyProfile.SicCodes.OrEmpty().Select(code => new Relationship<Company, SicCode>(companyProfileNode, _sicCodeReader.Read(code), "NATURE_OF_BUSINESS")));

        var officers = await _client.GetOfficersAsync(safeCompanyNumber, cancellationToken);

        foreach (var officer in officers)
        {
            if (OfficersToIgnore.Contains(officer.Links.Officer.OfficerId, StringComparer.OrdinalIgnoreCase) ||  RolesToIgnore.Contains(officer.OfficerRole))
            {
                Log.Information("Ignoring Officer: {@Officer}", new { Id = officer.Links.Officer.OfficerId, officer.Name, Role = officer.OfficerRole});
                continue;
            }
            
            var officerNode = new Officer(officer);

            var appointments = await _client.GetAppointmentsAsync(officer.Links.Officer.OfficerId, cancellationToken);
            
            foreach (var appointment in appointments.Where(x => x.Appointed.CompanyNumber.Equals(safeCompanyNumber) && !RolesToIgnore.Contains(x.OfficerRole)))
            {
                if (CompaniesToIgnore.Contains(appointment.Appointed.CompanyNumber, StringComparer.OrdinalIgnoreCase))
                {
                    Log.Information("Ignoring Appointment: {@Appointment}", new { OfficeId = officer.Links.Officer.OfficerId, officer.Name, appointment.Appointed.CompanyNumber, appointment.Appointed.CompanyName});
                    continue;
                }

                if (!companyProfileNode.Number.Equals(safeCompanyNumber)) continue;
                
                companyProfileNode.AddRelation(new Appointment(companyProfileNode, officerNode, appointment));
            }
        }
        
        await _session.ExecuteCommandsAsync(companyProfileNode);
    }

    public async Task<Company> CreateOfficersAndCompaniesAsync(string companyNumber, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(companyNumber)) return null;

        var safeCompanyNumber = companyNumber.PadLeft(8, '0');
        
        if (CompaniesToIgnore.Contains(safeCompanyNumber, StringComparer.OrdinalIgnoreCase))
        {
            Log.Information("Ignoring Company: {Number}", safeCompanyNumber);
            return null;
        }

        Company company = null;
        
        var officers = await _client.GetOfficersAsync(safeCompanyNumber, cancellationToken);

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
                companyProfileNode.AddRelation(new Appointment(companyProfileNode, officerNode, appointment));

                if (companyProfileNode.Number.Equals(safeCompanyNumber))
                {
                    company = companyProfileNode;
                }
                
                await _session.ExecuteCommandsAsync(companyProfileNode);
            }
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

    public Task DeleteAllAsync() => _session.DeleteAllAsync();
}