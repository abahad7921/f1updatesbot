using F1UpdatesBot.Src.Helpers;
using F1UpdatesBot.Src.Models;
using F1UpdatesBot.Src.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1UpdatesBot.Src.Services
{
    public class SessionsService : ISessionsService
    {
        private readonly HttpClient _httpClient;
        private int? _sessionKey;

        public SessionsService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.openf1.org/v1/")
            };
        }

        public async Task<List<Session>> GetAllAsync()
        {
            return await RetryHelper.ExecuteWithRetryAsync(async () =>
            {
                var year = DateTime.UtcNow.Year;
                var response = await _httpClient.GetStringAsync($"sessions?year={year}");
                return JsonConvert.DeserializeObject<List<Session>>(response);
            });
        }

        public async Task<Session?> GetNextRaceSessionAsync()
        {
            var now = DateTime.UtcNow;
            var sessions = await GetAllAsync();

            return sessions
                .Where(s => string.Equals(s.SessionType, "Race", StringComparison.OrdinalIgnoreCase))
                // Include a race already under way as well as the next future race.
                .Where(s => !s.DateEnd.HasValue || s.DateEnd.Value >= now)
                .OrderBy(s => s.DateStart)
                .FirstOrDefault();
        }

        public async Task<int> GetCurrentSessionKeyAsync()
        {
            if (_sessionKey is null)
            {
                var raceSession = await GetNextRaceSessionAsync();
                _sessionKey = raceSession?.SessionKey ?? 0;
            }

            return _sessionKey.Value;
        }
    }
}
