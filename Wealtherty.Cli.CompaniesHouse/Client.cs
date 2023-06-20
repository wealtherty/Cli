using CompaniesHouse;
using CompaniesHouse.Response.Appointments;
using CompaniesHouse.Response.Officers;
using Serilog;

namespace Wealtherty.Cli.CompaniesHouse;

public class Client
{
    private readonly ICompaniesHouseClient _companiesHouseClient;

    public Client(ICompaniesHouseClient companiesHouseClient)
    {
        _companiesHouseClient = companiesHouseClient;
    }

    public async Task<Officer[]> GetOfficersAsync(string companyNumber)
    {
        var getOfficersResponse = await _companiesHouseClient.GetOfficersAsync(companyNumber);
        
        Log.Information("Officers: {@Counts}", new { getOfficersResponse.Data.ActiveCount, getOfficersResponse.Data.ResignedCount, getOfficersResponse.Data.Items.Length});
        
        return getOfficersResponse.Data.Items;
    }

    public async Task<Appointment[]> GetAppointmentsAsync(string officerId)
    {
        var getAppointmentsResponse = await _companiesHouseClient.GetAppointmentsAsync(officerId);
            
        Log.Information("Appointments: {@Counts}", new { getAppointmentsResponse.Data.TotalResults, getAppointmentsResponse.Data.Items.Length});

        return getAppointmentsResponse.Data.Items;
    } 
}