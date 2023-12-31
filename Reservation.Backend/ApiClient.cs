﻿using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reservation.Backend.Models;
using Attribute = Reservation.Backend.Models.Attribute;

namespace Reservation.Backend;

public class ApiClient
{
    private static readonly JsonSerializerOptions WebJsonSerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiClient> _logger;

    public ApiClient(HttpClient httpClient, IOptions<ApiClientOptions> options, ILogger<ApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpClient.BaseAddress = new Uri($"{options.Value.HostUrl}/api/");
    }

    public async Task<List<Location>?> GetLocations(CancellationToken token = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("general/location/all", token);
            if (!response.IsSuccessStatusCode)
            {
                return new List<Location>();
            }

            var content = await response.Content.ReadAsStringAsync(token);
            return JsonSerializer.Deserialize<List<Location>>(content, WebJsonSerializerOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError("Couldn't get locations {Message}\n{Stack}", ex.Message, ex.StackTrace);
            return new List<Location>();
        }
    }

    public async Task<IEnumerable<DayOff>> GetDayOffsForNext30Days(CancellationToken token = default)
    {
        var response = await _httpClient.GetAsync("general/dayoff/next30days", token);
        if (!response.IsSuccessStatusCode)
        {
            return new List<DayOff>();
        }
        var content = await response.Content.ReadAsStreamAsync(token);
        var result = await JsonSerializer.DeserializeAsync<List<DayOff>>(content, WebJsonSerializerOptions, cancellationToken: token);
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
        var result =  await JsonSerializer.DeserializeAsync<List<Attribute>>(content, WebJsonSerializerOptions, cancellationToken: token);
        return result ?? new List<Attribute>();
    }

    public async Task<List<TimeSlot>> GetFreeTimeSlotsForTyreChange(TimeSlotRequest request, CancellationToken token = default)
    {
        var response = await _httpClient.PostAsync("general/free/free_tyre_change_date_time_by_criteria", JsonContent.Create(request), token);
        if (!response.IsSuccessStatusCode)
        {
            return new List<TimeSlot>();
        }
        var content = await response.Content.ReadAsStreamAsync(token);
        var result =  await JsonSerializer.DeserializeAsync<List<TimeSlot>>(content, WebJsonSerializerOptions, cancellationToken: token);
        return result ?? new List<TimeSlot>();
    }

    public async Task<WebReservationResponse> RegisterTyreChange(TyreChangeReservation request, CancellationToken token = default)
    {
        var response = await _httpClient.PostAsync("general/reservation/register_tyre_change", JsonContent.Create(request), token);
        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync(token);
            _logger.LogDebug("Error message: {ErrorMessage}", errorMessage);
            var error = JsonSerializer.Deserialize<BusinessError>(errorMessage);
            ThrowBusinessException(error);
        }
        var content = await response.Content.ReadAsStreamAsync(token);
        var result =  await JsonSerializer.DeserializeAsync<WebReservationResponse>(content, WebJsonSerializerOptions, cancellationToken: token);
        return result ?? new WebReservationResponse();
    }
    
    public async Task<WebReservationResponse> RegisterAirCondition(AirConditionReservation request, CancellationToken token = default)
    {
        var response = await _httpClient.PostAsync("general/reservation/register_clima_service", JsonContent.Create(request), token);
        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync(token);
            _logger.LogDebug("Error message: {ErrorMessage}", errorMessage);
            var error = JsonSerializer.Deserialize<BusinessError>(errorMessage);
            ThrowBusinessException(error);
        }
        var content = await response.Content.ReadAsStreamAsync(token);
        var result =  await JsonSerializer.DeserializeAsync<WebReservationResponse>(content, WebJsonSerializerOptions, cancellationToken: token);
        return result ?? new WebReservationResponse();
    }
    
    private void ThrowBusinessException(BusinessError? error)
    {
        if (error.items is not null && error.items.Count > 0)
        {
            var code = error.items[0].code;
            _logger.LogInformation("Code: {Code}", code);
            if (code is not null)
            {
                code = code.Remove(code.Length - 1);
                throw new BusinessException(code);
            }
        }

        throw new BusinessException("UnknownErrorCode");
    }
}

public class BusinessException : Exception
{
    public string Error { get; }

    public BusinessException(string error)
    {
        Error = error;
    }
}

public class BusinessError
{
    public List<BusinessErrorItem> items { get; set; }
}

public record BusinessErrorItem(string code, string description, BusinessErrorItem[] InnerErrors);