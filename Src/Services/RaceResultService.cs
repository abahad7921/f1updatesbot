using Discord;
using Discord.WebSocket;
using DotNetEnv;
using F1UpdatesBot.Src.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace F1UpdatesBot.Src.Services
{
    public class RaceResultService
    {
        private readonly Openf1Service _openF1Service;
        private IMessageChannel? _channel;
        private readonly DiscordSocketClient _client;
        private readonly TaskCompletionSource _channelReadyTcs = new();
        private ulong _channelId = 1367197611097850050;
        private readonly Dictionary<int, string> _driverMapping = new Dictionary<int, string>
    {
        { 1, "Max Verstappen" },
        { 2, "Logan Sargeant" },
        { 3, "Daniel Ricciardo" },
        { 4, "Lando Norris" },
        { 5, "Gabriel Bortoleto" },
        { 6, "Isack Hadjar" },
        { 7, "Jack Doohan" },
        { 10, "Pierre Gasly" },
        { 11, "Sergio Pérez" },
        { 12, "Andrea Kimi Antonelli" },
        { 14, "Fernando Alonso" },
        { 16, "Charles Leclerc" },
        { 18, "Lance Stroll" },
        { 20, "Kevin Magnussen" },
        { 22, "Yuki Tsunoda" },
        { 23, "Alexander Albon" },
        { 24, "Zhou Guanyu" },
        { 27, "Nico Hülkenberg" },
        { 30, "Liam Lawson" },
        { 31, "Esteban Ocon" },
        { 33, "Max Verstappen" },
        { 35, "Sergey Sirotkin" },
        { 43, "Franco Colapinto" },
        { 44, "Lewis Hamilton" },
        { 47, "Mick Schumacher" },
        { 53, "Alexander Rossi" },
        { 55, "Carlos Sainz Jr." },
        { 63, "George Russell" },
        { 77, "Valtteri Bottas" },
        { 81, "Oscar Piastri" },
        { 87, "Oliver Bearman" },
        { 88, "Rio Haryanto" },
        { 94, "Pascal Wehrlein" },
        { 98, "Roberto Merhi" },
        { 99, "Adrian Sutil" }
    };


        public RaceResultService(Openf1Service openF1Service, DiscordSocketClient client)
        {
            Env.Load();

            _openF1Service = openF1Service;
            _client = client;
            _client.Ready += OnClientReady;
            string channelIdString = Environment.GetEnvironmentVariable("CHANNEL_ID");
            if (ulong.TryParse(channelIdString, out ulong channelId))
            {
                // Successfully parsed
                _channelId = channelId;
            }
            else
            {
                Console.WriteLine("Invalid or missing CHANNEL_ID environment variable.");
            }
        }

        private Task OnClientReady()
        {
            _channel = _client.GetChannel(_channelId) as IMessageChannel;
            _channelReadyTcs.SetResult();
            Console.WriteLine($"Channel ready: {_channel != null}");
            return Task.CompletedTask;
        }

        public async Task SendFinalRaceStandingsAsync()
        {
            // Fetch position data (replace "GetPositionsAsync" with the actual OpenF1 endpoint)
            var positions = await _openF1Service.GetDriverPositionsAsync();
            if (positions == null || !positions.Any()) return;

            // Filter for the final classification (last recorded position for each driver)
            var finalStandings = positions
                .OrderByDescending(p => p.Date)
                .GroupBy(p => p.DriverNumber)
                .Select(g =>
                {
                    var finalPosition = g.OrderByDescending(p => p.LapNumber).FirstOrDefault();
                    return new DriverStanding
                    {
                        DriverNumber = g.Key,
                        Position = finalPosition?.CurrentPosition ?? 0
                    };
                })
                .OrderBy(ds => ds.Position) // Lower position number = better ranking
                .ToList();

            // Build the message
            var message = new StringBuilder("**🏁 Final Race Standings 🏁**\n\n");
            foreach (var driver in finalStandings)
            {
                var driverName = GetDriverName(driver.DriverNumber);
                message.AppendLine($"{driver.Position}. {driverName} ({driver.DriverNumber})");
            }

            await _channel.SendMessageAsync(message.ToString());
        }

        public class DriverStanding
        {
            public int DriverNumber { get; set; }
            public int Position { get; set; }

        }

        private string GetDriverName(int driverNumber)
        {
            return _driverMapping.ContainsKey(driverNumber) ? _driverMapping[driverNumber] : $"Driver {driverNumber}";
        }
    }
}