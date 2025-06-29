using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using FitnessTracker.Models;

namespace FitnessTracker.Services
{
    public class WgerService : IWgerService
    {
        private readonly HttpClient _client;
        public WgerService(HttpClient client) => _client = client;

        public async Task<IEnumerable<ExerciseDto>> GetExercisesAsync(int page = 1)
        {
            // Use the exerciseinfo endpoint which includes translations.
            // translation contains the exercise name for a specific language.
            var url = $"/api/v2/exerciseinfo/?language=2&status=2&page={page}&format=json";

            try
            {
                var httpResp = await _client.GetAsync(url);
                if (!httpResp.IsSuccessStatusCode)
                    return new List<ExerciseDto>();

                var json = await httpResp.Content.ReadAsStringAsync();
                var resp = JsonSerializer.Deserialize<WgerResponse<ExerciseInfoDto>>(json);
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
            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException || ex is JsonException)
            {
                Console.Error.WriteLine($"Wger error: {ex.Message}");
                return new List<ExerciseDto>();
            }
        }

    }
}
