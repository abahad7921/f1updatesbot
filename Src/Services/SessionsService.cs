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

        public async Task<List<Session>> getAll()
        {
            return await RetryHelper.ExecuteWithRetryAsync(async () =>
            {
                var response = await _httpClient.GetStringAsync("sessions?year=2025");
                return JsonConvert.DeserializeObject<List<Session>>(response);
            });
        }

        public async Task<int> getCurrentSessionKey()
        {   
            if (_sessionKey == null)
            {
                var sessions = await getAll();
                var upcomingRace = sessions
                     .Where(s => s.DateStart > DateTime.UtcNow)
                       .OrderByDescending(s => s.DateStart)
                    .FirstOrDefault();

                _sessionKey = upcomingRace == null ? 0 : upcomingRace.SessionKey;
            }

            return _sessionKey.Value;
        }
    }
}
