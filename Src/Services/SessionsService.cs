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

        public SessionsService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.openf1.org/v1/")
            };
        }

        public async Task<List<Session>> getAll()
        {
            var response = await _httpClient.GetStringAsync("sessions?year=2025&session_name=Race");
            return JsonConvert.DeserializeObject<List<Session>>(response);
        }

        public async Task<int> getCurrentSessionKey()
        {
            var sessions = await getAll();
            var upcomingRace = sessions
                .Where(s => s.SessionType == "Race" && s.DateStart > DateTime.UtcNow)
                   .OrderBy(s => s.DateStart)
                .FirstOrDefault();

            return upcomingRace == null ? 0 : upcomingRace.SessionKey;
        }
    }
}
