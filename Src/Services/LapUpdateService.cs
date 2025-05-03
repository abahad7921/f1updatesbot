using Discord;
using Discord.WebSocket;
using DotNetEnv;
using F1UpdatesBot.Src.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace F1UpdatesBot.Src.Services
{
    // Services/LapUpdateService.cs
    public class LapUpdateService
    {
        private readonly Openf1Service _openF1Service;
        private IMessageChannel? _channel;
        private readonly Dictionary<int, int> _driverLastLap = new();
        private readonly DiscordSocketClient _client;
        private readonly TaskCompletionSource _channelReadyTcs = new();
        private ulong _channelId;
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

        public LapUpdateService(Openf1Service openF1Service, DiscordSocketClient client)
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

        public async Task SendLapUpdatesAsync(int sessionKey = 10022)
        {
            try
            {
                // Fetch data directly from the OpenF1 API
                var laps = await _openF1Service.GetLapsAsync();

                // Get driver positions from the same API
                var driverInfo = await _openF1Service.GetDriverPositionsAsync();

                if (laps == null || driverInfo == null || !laps.Any() || !driverInfo.Any())
                {
                    await _channel.SendMessageAsync("Could not retrieve F1 race data");
                    return;
                }

                // Build a lookup for current positions
                var positionLookup = driverInfo.ToDictionary(
                    d => d.DriverNumber,
                    d => d.CurrentPosition);

                var driverData = new List<DriverLapInfo>();

                // Group laps by driver and get the latest lap for each
                var latestLapsByDriver = laps
                    .GroupBy(l => l.DriverNumber)
                    .ToDictionary(
                        g => g.Key,
                        g => g.OrderByDescending(l => l.LapNumber).FirstOrDefault()
                    );

                // Build combined data set
                foreach (var kvp in latestLapsByDriver)
                {
                    var driverNumber = kvp.Key;
                    var latestLap = kvp.Value;

                    if (latestLap == null) continue;

                    // Try to get position from our lookup, default to 0 if not found
                    positionLookup.TryGetValue(driverNumber, out var position);

                    driverData.Add(new DriverLapInfo
                    {
                        Position = position,
                        LapNumber = latestLap.LapNumber,
                        DriverNumber = driverNumber,
                        LapTime = latestLap.LapTime
                    });
                }

                // Primary sort by lap number (descending), then by position (ascending)
                var sortedData = driverData
                    .OrderByDescending(d => d.LapNumber)
                    .ThenBy(d => d.Position)
                    .ToList();

                bool hasNewLaps = false;
                var message = new StringBuilder("**Current Race Standings**\n");

                // First, include leaders on current lap
                var maxLap = sortedData.Any() ? sortedData.Max(d => d.LapNumber) : 0;
                var leaders = sortedData.Where(d => d.LapNumber == maxLap).ToList();

                if (leaders.Any())
                {
                    message.AppendLine($"\n**Leaders (Lap {maxLap})**");
                    foreach (var driver in leaders)
                    {
                        if (driver.LapNumber == GetLastLap(driver.DriverNumber)) continue;

                        hasNewLaps = true;
                        var driverName = GetDriverName(driver.DriverNumber);
                        var lapTimeDisplay = string.IsNullOrEmpty(driver.LapTime)
                            ? "In Progress"
                            : driver.LapTime;

                        message.AppendLine(
                            $"{driver.Position}. {driverName} " +
                            $"- {lapTimeDisplay}");

                        SetLastLap(driver.DriverNumber, driver.LapNumber);
                    }
                }

                // Then, include drivers who are lapped
                var lappedDrivers = sortedData.Where(d => d.LapNumber < maxLap).ToList();

                if (lappedDrivers.Any())
                {
                    message.AppendLine($"\n**Lapped Drivers**");
                    foreach (var driver in lappedDrivers)
                    {
                        if (driver.LapNumber == GetLastLap(driver.DriverNumber)) continue;

                        hasNewLaps = true;
                        var driverName = GetDriverName(driver.DriverNumber);
                        var lapsBehind = maxLap - driver.LapNumber;
                        var lapsBehindText = lapsBehind == 1 ? "1 lap behind" : $"{lapsBehind} laps behind";

                        message.AppendLine(
                            $"{driver.Position}. {driverName} " +
                            $"- Lap {driver.LapNumber} " +
                            $"({lapsBehindText})");

                        SetLastLap(driver.DriverNumber, driver.LapNumber);
                    }
                }

                if (hasNewLaps)
                    await _channel.SendMessageAsync(message.ToString());
                else
                    await _channel.SendMessageAsync("No new lap updates");
            }
            catch (Exception ex)
            {
                await _channel.SendMessageAsync($"Error retrieving F1 race data: {ex.Message}");
            }
        }

        // Helper class
        private class DriverLapInfo
        {
            public int Position { get; set; }
            public int LapNumber { get; set; }
            public int DriverNumber { get; set; }
            public string LapTime { get; set; }
        }

        private int GetLastLap(int driverNumber)
        {
            return _driverLastLap.ContainsKey(driverNumber) ? _driverLastLap[driverNumber] : 0;
        }

        private void SetLastLap(int driverNumber, int lapNumber)
        {
            if (_driverLastLap.ContainsKey(driverNumber))
                _driverLastLap[driverNumber] = lapNumber;
            else
                _driverLastLap.Add(driverNumber, lapNumber);
        }

        private string GetDriverName(int driverNumber)
        {
            return _driverMapping.ContainsKey(driverNumber) ? _driverMapping[driverNumber] : $"Driver {driverNumber}";
        }

    }

}
