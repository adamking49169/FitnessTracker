using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FitnessTracker.Models;
using System.Text.Json;

namespace FitnessTracker.Services
{
    public class WgerService : IWgerService
    {
        private readonly HttpClient _client;
        public WgerService(HttpClient client) => _client = client;

        public async Task<IEnumerable<ExerciseDto>> GetExercisesAsync(int page = 1)
        {
            // Call the /exercise/ endpoint (not exerciseinfo)
            // Use the exerciseinfo endpoint to get translated names
            var url = $"/api/v2/exercise/?language=2&status=2&page={page}&format=json";

            var httpResp = await _client.GetAsync(url);
            Console.WriteLine($"Status: {httpResp.StatusCode}");
            string json = string.Empty;
            if (httpResp.IsSuccessStatusCode)
            {
                json = await httpResp.Content.ReadAsStringAsync();
                Console.WriteLine(json);
            }

            var resp = string.IsNullOrEmpty(json)
               ? null
               : JsonSerializer.Deserialize<WgerResponse<ExerciseDto>>(json);

            if (resp == null) return new List<ExerciseDto>();

            return resp.Results
                      .Select(e =>
                      {
                          if (string.IsNullOrWhiteSpace(e.Name))
                              e.Name = e.NameOriginal;
                          return e;
                      })
                      .Where(e => !string.IsNullOrWhiteSpace(e.Name))
                      .ToList();
        }

    }
}
