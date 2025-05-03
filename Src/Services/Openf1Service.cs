using F1UpdatesBot.Src.Helpers;
using F1UpdatesBot.Src.Models;
using F1UpdatesBot.Src.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace F1UpdatesBot.Src.Services
{
    public class Openf1Service: IOpenf1Service
    {
        private readonly HttpClient _httpClient;
        private readonly SessionsService _sessionsService;
        private readonly int _sessionKey;

        public Openf1Service(SessionsService sessionsService, HttpClient client)
        {   
            _httpClient = client;
            _sessionsService = sessionsService;
            _sessionKey = _sessionsService.getCurrentSessionKey().GetAwaiter().GetResult();
        }

        public async Task<List<Driver>> GetDriversAsync()
        {
            return await RetryHelper.ExecuteWithRetryAsync(async () =>
            {
                var response = await _httpClient.GetStringAsync($"drivers?session_key={_sessionKey}");
                return JsonConvert.DeserializeObject<List<Driver>>(response);
            });
        }

        public async Task<List<Lap>> GetLapsAsync()
        {
            return await RetryHelper.ExecuteWithRetryAsync(async () =>
            {
                var response = await _httpClient.GetStringAsync($"laps?session_key={_sessionKey}");
                return JsonConvert.DeserializeObject<List<Lap>>(response);
            });
        }

        public async Task<List<Position>> GetDriverPositionsAsync()
        {
            return await RetryHelper.ExecuteWithRetryAsync(async () =>
            {
                var response = await _httpClient.GetStringAsync($"position?session_key={_sessionKey}");
                return JsonConvert.DeserializeObject<List<Position>>(response);
            });
        }

    }
}

