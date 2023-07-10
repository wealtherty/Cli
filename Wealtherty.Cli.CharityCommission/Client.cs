using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Wealtherty.Cli.CharityCommission.Model;

namespace Wealtherty.Cli.CharityCommission;

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

    public async Task<Chairty> GetDetailsAsync(string registeredNumber)
    {
        var response = await _httpClient.GetAsync($"/register/api/charitydetails/{registeredNumber}/0");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        
        return JsonConvert.DeserializeObject<Chairty>(json);
    }

    public async Task<Trustee[]> GetTrusteesAsync(string registeredNumber)
    {
        var response = await _httpClient.GetAsync($"/register/api/charitytrusteeinformation/{registeredNumber}/0");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        
        return JsonConvert.DeserializeObject<Trustee[]>(json);
    }
}