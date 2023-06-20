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
        
        Log.Information("Officers - CompanyNumber: {CompanyNumber}, Counts: {@Counts}", companyNumber, new { Expected =  getOfficersResponse.Data.ActiveCount + getOfficersResponse.Data.ResignedCount, Actual = getOfficersResponse.Data.Items.Length});
        
        return getOfficersResponse.Data.Items;
    }

    public async Task<Appointment[]> GetAppointmentsAsync(string officerId)
    {
        const int pageSize = 50;
        
        CompaniesHouseClientResponse<Appointments> getAppointmentsResponse;
        var appointmemts = new List<Appointment>();
        var startIndex = 0;

        do
        {
            getAppointmentsResponse = await _companiesHouseClient.GetAppointmentsAsync(officerId, startIndex, pageSize);
            appointmemts.AddRange(getAppointmentsResponse.Data.Items);
            startIndex += pageSize;
        } while (appointmemts.Count != getAppointmentsResponse.Data.TotalResults);
        
        Log.Information("Appointments - OfficerId: {OfficerId}, Counts: {@Counts}",
            officerId,
            new { Expected = getAppointmentsResponse.Data.TotalResults, Actual = appointmemts.Count });
        
        return appointmemts.ToArray();
    } 
}