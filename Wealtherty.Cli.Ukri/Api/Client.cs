using System.Net.Http.Headers;
using Newtonsoft.Json;
using Wealtherty.Cli.Ukri.Api.Model;

namespace Wealtherty.Cli.Ukri.Api;

public class Client : IDisposable
{
    private readonly HttpClient _httpClient;

    public Client()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://gtr.ukri.org"),
        };

        _httpClient.DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/vnd.rcuk.gtr.json-v7"));

    }
    
    public async Task<Project[]> SearchProjectsAsync(string query, int size = 100)
    {
        var page = 0;
        var projects = new List<Project>();
        var searchProjectsResponse = new SearchProjectsResponse();
        do
        {
            page += 1;
            
            var response = await _httpClient.GetAsync($"/gtr/api/projects?q={query}&s={size}&p={page}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            searchProjectsResponse = JsonConvert.DeserializeObject<SearchProjectsResponse>(json);
            projects.AddRange(searchProjectsResponse.Projects);
        } while (page != searchProjectsResponse.TotalPages);

        return projects.ToArray();
    }
    
    public Task<Fund> GetFundAsync(string id) => GetAsync<Fund>($"/gtr/api/funds/{id}");


    public Task<Organisation> GetOrganisationAsync(string id) => GetAsync<Organisation>($"/gtr/api/organisations/{id}");

    public Task<Person> GetPersonAsync(string id) => GetAsync<Person>($"/gtr/api/persons/{id}");

    private async Task<T> GetAsync<T>(string uri)
    {
        var response = await _httpClient.GetAsync(uri);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<T>(json);
    }


    public void Dispose() => _httpClient?.Dispose();

}