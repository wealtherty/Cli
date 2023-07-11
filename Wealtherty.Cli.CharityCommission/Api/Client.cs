using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using Wealtherty.Cli.CharityCommission.Api.Model;

namespace Wealtherty.Cli.CharityCommission.Api;

public class Client
{
    private readonly HttpClient _httpClient;

    public Client(IOptions<Settings> settings)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(settings.Value.Uri)
        };
        _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", settings.Value.SubscriptionKey);
    }

    public async Task<Charity> GetDetailsAsync(string number, CancellationToken cancellationToken)
    {
        Log.Debug("Getting Charity - Number: {Number}", number);
        
        var response = await _httpClient.GetAsync($"/register/api/charitydetails/{number}/0", cancellationToken);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        var chairty = JsonConvert.DeserializeObject<Charity>(json);
        
        Log.Debug("Got Charity - Number: {Number}, Charity: {@Charity}", number, chairty);
        
        return chairty;
    }

    public async Task<Trustee[]> GetTrusteesAsync(string number, CancellationToken cancellationToken)
    {
        Log.Debug("Getting Trustees - Number: {Number}", number);
        
        var response = await _httpClient.GetAsync($"/register/api/charitytrusteeinformation/{number}/0", cancellationToken);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        var trustees = JsonConvert.DeserializeObject<Trustee[]>(json);
        
        Log.Debug("Got Trustees - Number: {Number}, Trustees: {@Trustees}", number, trustees);
        
        return trustees;
    }
}