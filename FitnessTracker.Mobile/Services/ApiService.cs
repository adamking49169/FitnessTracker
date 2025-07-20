using System.Net.Http.Json;
using FitnessTracker.Mobile.Models;

namespace FitnessTracker.Mobile.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Example: login - placeholder implementation
    public async Task<bool> LoginAsync(string username, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("api/login", new { username, password });
        return response.IsSuccessStatusCode;
    }

    public async Task<List<WorkoutDto>?> GetWorkoutsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<WorkoutDto>>("api/workouts");
    }
}
