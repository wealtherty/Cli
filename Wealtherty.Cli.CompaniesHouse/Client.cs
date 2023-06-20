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
        const int pageSize = 50;
        
        var officers = new List<Officer>();
        var startIndex = 0;
        int expected;

        do
        {
            var response = await _companiesHouseClient.GetOfficersAsync(companyNumber, startIndex, pageSize);
            officers.AddRange(response.Data.Items);
            expected = response.Data.ActiveCount.Value + response.Data.ResignedCount.Value;
            startIndex += pageSize;

        } while (officers.Count != expected);

        Log.Information("GetOfficers - CompanyNumber: {CompanyNumber}, Counts: {@Counts}", 
            companyNumber, new { Expected = expected, Actual = officers.Count });
        
        return officers.ToArray();
    }

    public async Task<Appointment[]> GetAppointmentsAsync(string officerId)
    {
        const int pageSize = 50;
        
        var appointmemts = new List<Appointment>();
        var startIndex = 0;
        int expected;

        do
        {
            var response = await _companiesHouseClient.GetAppointmentsAsync(officerId, startIndex, pageSize);
            appointmemts.AddRange(response.Data.Items);
            expected = response.Data.TotalResults;
            startIndex += pageSize;
        } while (appointmemts.Count != expected);

        Log.Information("GetAppointments - OfficerId: {OfficerId}, Counts: {@Counts}",
            officerId, new { Expected = expected, Actual = appointmemts.Count });
        
        return appointmemts.ToArray();
    } 
}