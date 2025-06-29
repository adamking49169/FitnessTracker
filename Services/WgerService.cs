using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
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
            // Use the exerciseinfo endpoint which includes translations. Each
            // translation contains the exercise name for a specific language.
            var url = $"/api/v2/exerciseinfo/?language=2&status=2&page={page}&format=json";

            // diagnostic call to inspect the raw response
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
                : JsonSerializer.Deserialize<WgerResponse<ExerciseInfoDto>>(json);
            if (resp == null) return new List<ExerciseDto>();

            return resp.Results
                      .Select(r =>
                      {
                          var t = r.Translations.FirstOrDefault(tr => tr.Language == 2);
                          return t == null
                              ? null
                              : new ExerciseDto { Id = r.Id, Name = t.Name, NameOriginal = t.Name };
                      })
                      .Where(e => e != null && !string.IsNullOrWhiteSpace(e.Name))
                      .ToList()!;
        }

    }
}
