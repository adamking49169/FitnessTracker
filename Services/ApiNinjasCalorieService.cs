// Services/ApiNinjasCalorieService.cs
using FitnessTracker.Models;
using FitnessTracker.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class ApiNinjasCalorieService : ICalorieService
{
    private readonly HttpClient _client;
    private readonly string _apiKey;

    public ApiNinjasCalorieService(HttpClient client, IConfiguration config)
    {
        _client = client;
        _apiKey = config["ApiNinjas:Key"];
        // e.g. in appsettings.json:
        // "ApiNinjas": { "Key": "YOUR_KEY_HERE" }

        // BaseAddress can be set in DI
    }

    public async Task<int> GetCaloriesBurnedAsync(string activity, int weightKg, int durationMin)
    {
        var url = $"v1/caloriesburned?activity={Uri.EscapeDataString(activity)}"
                  + $"&weight={weightKg}&duration={durationMin}";
        using var req = new HttpRequestMessage(HttpMethod.Get, url);
        req.Headers.Add("X-Api-Key", _apiKey);

        try
        {
            using var res = await _client.SendAsync(req);
            if (!res.IsSuccessStatusCode)
                return 0;

            var json = await res.Content.ReadAsStringAsync();
            var list = JsonSerializer.Deserialize<List<CaloriesResult>>(json);
            if (list == null || list.Count == 0)
                return 0;

            return list[0]?.total_calories ?? 0;
        }
        catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException || ex is JsonException)
        {
            Console.Error.WriteLine($"ApiNinjas error: {ex.Message}");
            return 0;
        }
    }
    public async Task<IEnumerable<string>> GetActivitiesAsync()
    {
        const string url = "v1/caloriesburnedactivities";
        using var req = new HttpRequestMessage(HttpMethod.Get, url);
        req.Headers.Add("X-Api-Key", _apiKey);

        try
        {
            using var res = await _client.SendAsync(req);
            if (!res.IsSuccessStatusCode)
                return new List<string>();

            var json = await res.Content.ReadAsStringAsync();
            var list = JsonSerializer.Deserialize<List<string>>(json);
            return list ?? new List<string>();
        }
        catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException || ex is JsonException)
        {
            Console.Error.WriteLine($"ApiNinjas error: {ex.Message}");
            return new List<string>();
        }
    }
}
