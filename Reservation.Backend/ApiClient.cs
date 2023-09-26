using System.Text.Json;
using Microsoft.Extensions.Options;
using Reservation.Backend.Models;
using Attribute = Reservation.Backend.Models.Attribute;

namespace Reservation.Backend;

public class ApiClient
{
    private readonly HttpClient _httpClient;

    public ApiClient(HttpClient httpClient, IOptions<ApiClientOptions> options)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri($"{options.Value.HostUrl}/api/");
    }

    public async Task<List<Location>?> GetLocations(CancellationToken token = default)
    {
        var response = await _httpClient.GetAsync("general/location/all", token);
        if (!response.IsSuccessStatusCode)
        {
            return new List<Location>();
        }

        var content = await response.Content.ReadAsStreamAsync(token);
        return await JsonSerializer.DeserializeAsync<List<Location>>(content, cancellationToken: token);
    }

    public async Task<IEnumerable<DayOff>> GetDayOffsForNext30Days(CancellationToken token = default)
    {
        var response = await _httpClient.GetAsync("general/dayoff/next30days", token);
        if (!response.IsSuccessStatusCode)
        {
            return new List<DayOff>();
        }
        var content = await response.Content.ReadAsStreamAsync(token);
        var result = await JsonSerializer.DeserializeAsync<List<DayOff>>(content, cancellationToken: token);
        return result ?? new List<DayOff>();
    }

    public async Task<IEnumerable<Attribute>> ListForObjectNameAndObjectId(string objectName, int locationId, CancellationToken token = default)
    {
        var response = await _httpClient.GetAsync("general/attribute/list", token);
        if (!response.IsSuccessStatusCode)
        {
            return new List<Attribute>();
        }
        var content = await response.Content.ReadAsStreamAsync(token);
        var result =  await JsonSerializer.DeserializeAsync<List<Attribute>>(content, cancellationToken: token);
        return result ?? new List<Attribute>();
    }
}