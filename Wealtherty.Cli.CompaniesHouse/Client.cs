using CompaniesHouse;
using CompaniesHouse.Request;
using CompaniesHouse.Response.Appointments;
using CompaniesHouse.Response.CompanyProfile;
using CompaniesHouse.Response.Officers;
using Polly;
using Polly.Retry;
using Serilog;
using Serilog.Events;

namespace Wealtherty.Cli.CompaniesHouse;

public class Client
{
    private const int PageSize = 50;
    private static readonly SemaphoreSlim Semaphore = new(1,1);

    private static readonly AsyncRetryPolicy Retry = Policy
        .Handle<HttpRequestException>(x => !x.Message.Contains("404"))
        .WaitAndRetryAsync(Enumerable.Range(0, 30).Select(_ => TimeSpan.FromMinutes(1)),
            (exception, span) => Log.Warning(exception, "Error calling Companies House - Sleeping for: {@Span}", span));

    private readonly ICompaniesHouseClient _companiesHouseClient;

    public Client(ICompaniesHouseClient companiesHouseClient)
    {
        _companiesHouseClient = companiesHouseClient;
    }

    public async Task<Officer[]> GetOfficersAsync(string companyNumber, CancellationToken cancellationToken = new())
    {
        await Semaphore.WaitAsync(cancellationToken);

        Log.Debug("Getting Company Officers - CompanyNumber: {CompanyNumber}", companyNumber);

        try
        {
            var officers = new List<Officer>();
            var startIndex = 0;
            int expected;

            do
            {
                var policyResult = await Retry.ExecuteAndCaptureAsync(() => _companiesHouseClient.GetOfficersAsync(companyNumber, startIndex, PageSize, cancellationToken));
                var response = policyResult.Result;

                if (response.Data?.Items == null)
                {
                    throw new Exception($"Error getting officers.  Response items is null - CompanyNumber: {companyNumber}");
                }
            
                officers.AddRange(response.Data.Items);
                expected = response.Data.ActiveCount ?? 0 + response.Data.ResignedCount?? 0;
                startIndex += PageSize;

            } while (officers.Count < expected);

            Log.Debug("Company Officers - CompanyNumber: {CompanyNumber}, Counts: {@Counts}", companyNumber,
                new { Expected = expected, Actual = officers.Count });
            return officers.ToArray();
        }
        finally
        {
            Semaphore.Release();
        }
    }

    public async Task<Appointment[]> GetAppointmentsAsync(string officerId, CancellationToken cancellationToken = new(), int maxResults = 1000)
    {
        await Semaphore.WaitAsync(cancellationToken);
        
        Log.Debug("Getting Officer Appointments - OfficerId: {OfficerId}", officerId);

        try
        {
            var appointmemts = new List<Appointment>();
            var startIndex = 0;
            int expected;

            do
            {
                var policyResult = await Retry.ExecuteAndCaptureAsync(() => _companiesHouseClient.GetAppointmentsAsync(officerId, startIndex, PageSize, cancellationToken));
                var response = policyResult.Result;

                appointmemts.AddRange(response.Data.Items);
                expected = response.Data.TotalResults;
                startIndex += PageSize;
            } while (expected < maxResults && appointmemts.Count != expected);

            Log.Write(expected > maxResults ? LogEventLevel.Warning : LogEventLevel.Debug, "Officer Appointments - OfficerId: {OfficerId}, Counts: {@Counts}", officerId,
                new { Max = maxResults, Expected = expected, Actual = appointmemts.Count });
            
            return appointmemts.ToArray();
        }
        finally
        {
            Semaphore.Release();
        }
    }

    public async Task<CompaniesHouseClientResponse<CompanyProfile>> GetCompanyProfileAsync(string companyNumber, CancellationToken cancellationToken = default(CancellationToken))
    {
        await Semaphore.WaitAsync(cancellationToken);

        try
        {
            var policyResult = await Retry.ExecuteAndCaptureAsync(() => _companiesHouseClient.GetCompanyProfileAsync(companyNumber, cancellationToken));
            var response = policyResult.Result;
        
            return response;
        }
        finally
        {
            Semaphore.Release();
        }
    }

}