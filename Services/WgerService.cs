// Services/WgerService.cs
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FitnessTracker.Models;

namespace FitnessTracker.Services
{
    public class WgerService : IWgerService
    {
        private readonly HttpClient _client;
        public WgerService(HttpClient client) => _client = client;

        public async Task<IEnumerable<ExerciseDto>> GetExercisesAsync(int page = 1)
        {
            // now calling the /exerciseinfo/ endpoint which returns "name" for each exercise:
            var url = $"/api/v2/exerciseinfo/?language=2&page={page}&format=json";

            var resp = await _client.GetFromJsonAsync<WgerResponse<ExerciseDto>>(url);
            return resp?.Results ?? new List<ExerciseDto>();
        }
    }
}
