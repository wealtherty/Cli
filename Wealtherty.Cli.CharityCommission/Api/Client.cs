using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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

    public async Task<Chairty> GetDetailsAsync(string number)
    {
        var response = await _httpClient.GetAsync($"/register/api/charitydetails/{number}/0");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        
        return JsonConvert.DeserializeObject<Chairty>(json);
    }

    public async Task<Trustee[]> GetTrusteesAsync(string number)
    {
        var response = await _httpClient.GetAsync($"/register/api/charitytrusteeinformation/{number}/0");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        
        return JsonConvert.DeserializeObject<Trustee[]>(json);
    }
}