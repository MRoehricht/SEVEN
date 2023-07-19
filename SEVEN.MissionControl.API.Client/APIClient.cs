using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SEVEN.Core.Models;
using SEVEN.Core.Models.Configuration;

namespace SEVEN.Core.API.Client;

public class APIClient : IAPIClient
{
    private readonly string _baseUrl;

    public APIClient(IOptions<APIConnection> options)
    {
        if (options?.Value?.BaseUrl == null) throw new ArgumentNullException(nameof(APIConnection));

        _baseUrl = options.Value.BaseUrl;
    }

    public async Task<Rover?> GetRover(Guid roverId)
    {
        using var handler = new HttpClientHandler();
        using var client = new HttpClient(handler);

        client.BaseAddress = new Uri(_baseUrl);
        var response = await client.GetAsync("/rover/" + roverId);

        if (response != null)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Rover>(jsonString);
        }

        return null;
    }

    public async Task<RoverTask?> GetRoverTask(Guid taskId)
    {
        using var handler = new HttpClientHandler();
        using var client = new HttpClient(handler);

        client.BaseAddress = new Uri(_baseUrl);
        var response = await client.GetAsync("/tasks/" + taskId);

        if (response != null)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<RoverTask>(jsonString);
        }

        return null;
    }

    public async Task<IEnumerable<RoverTask>> GetReadyRoverTasks(Guid roverId)
    {
        using var handler = new HttpClientHandler();
        using var client = new HttpClient(handler);

        client.BaseAddress = new Uri(_baseUrl);
        var response = await client.GetAsync("/tasks/ready/" + roverId);

        if (response != null)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<RoverTask>>(jsonString) ?? new List<RoverTask>();
        }

        return new List<RoverTask>();
    }

    public async Task CreateRoverTask(RoverTask roverTask)
    {
        using var handler = new HttpClientHandler();
        using var client = new HttpClient(handler);

        client.BaseAddress = new Uri(_baseUrl);
        var response = await client.PostAsJsonAsync("/tasks", roverTask);
    }

    public async Task UpdateRoverTaskStatus(RoverTask roverTask)
    {
        using var handler = new HttpClientHandler();
        using var client = new HttpClient(handler);

        client.BaseAddress = new Uri(_baseUrl);
        var response = await client.PutAsJsonAsync("/tasks", roverTask);
        response?.EnsureSuccessStatusCode();
    }

    public async Task<ProbeToken?> GetProbeToken(Guid probeId)
    {
        using var handler = new HttpClientHandler();
        using var client = new HttpClient(handler);

        client.BaseAddress = new Uri(_baseUrl);
        var response = await client.GetAsync("/authentication?id=" + probeId);

        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ProbeToken>(jsonString);
        }

        return null;
    }

    public async Task<Measurement?> CreateMeasurement(Measurement measurement, ProbeToken token)
    {
        using var handler = new HttpClientHandler();
        using var client = new HttpClient(handler);
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token.Token);

        client.BaseAddress = new Uri(_baseUrl);
        var response = await client.PostAsJsonAsync("/measurement", measurement);
        
        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Measurement>(jsonString);
        }

        return null;
    }
}