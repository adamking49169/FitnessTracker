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
            var url = $"/api/v2/exerciseinfo/?language=2&page={page}&format=json";
            var resp = await _client.GetFromJsonAsync<WgerResponse<ExerciseDto>>(url);

            return resp != null
                ? resp.Results                 // List<ExerciseDto>
                : Array.Empty<ExerciseDto>();  // ExerciseDto[]
        }

    }
}
