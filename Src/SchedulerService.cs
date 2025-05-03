using Discord.WebSocket;
using Discord;
using F1UpdatesBot.Src.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace F1UpdatesBot.Src
{ 
    public class SchedulerService
    {
        private readonly DiscordSocketClient _client;
        private readonly Openf1Service _openF1Service;
        private readonly SessionsService _sessionsService;
        private readonly LapUpdateService _lapUpdateService;
        private readonly RaceResultService _raceResultService;
        private readonly DriverService _driverService;

        private Timer _lapUpdateTimer;
        private bool _raceStarted = false;
        private bool _resultSent = false;

        public SchedulerService(
            DiscordSocketClient client,
            Openf1Service openF1Service,
            SessionsService sessionsService,
            LapUpdateService lapUpdateService,
            RaceResultService raceResultService,
            DriverService driverService)
        {
            _client = client;
            _openF1Service = openF1Service;
            _sessionsService = sessionsService;
            _lapUpdateService = lapUpdateService;
            _raceResultService = raceResultService;
            _driverService = driverService;
        }

        public async Task StartAsync()
        {
            //await _driverService.SednDriverLineUpAsync("Riyadh");
            
            var sessions = await _sessionsService.getAll();
            var raceSession = sessions
                //.Where(s => s.SessionType == "Race")
                .Where(s => s.DateStart > DateTime.UtcNow)
                .OrderBy(s => s.DateStart)
                .FirstOrDefault();

            if (raceSession == null)
                return;

            var raceStart = raceSession.DateStart;
            //var raceEnd = raceSession.DateEnd ?? raceStart.AddHours(2); // default to 2 hours
            var raceEnd = raceSession.DateEnd;

            var channel = _client.GetChannel(ulong.Parse("1367197611097850050")) as IMessageChannel;

            // Schedule driver lineup 30 minutes before race
            _ = Task.Run(async () =>
            {
                var lineupDelay = raceStart.AddMinutes(-30) - DateTime.Now;
                if (lineupDelay > TimeSpan.Zero)
                    await Task.Delay(lineupDelay);

                var drivers = await _openF1Service.GetDriversAsync();
                string message = $"**Driver Lineup for {raceSession.Location} Grand Prix:**\n";
                foreach (var driver in drivers)
                {
                    message += $"- {driver.FullName} ({driver.TeamName})\n";
                }

                await channel.SendMessageAsync(message);
            });

            // Start lap updates at race start
            _ = Task.Run(async () =>
            {
                var raceDelay = raceStart - DateTime.Now;
                if (raceDelay > TimeSpan.Zero)
                    await Task.Delay(raceDelay);

                _raceStarted = true;
                _lapUpdateTimer = new Timer(async _ =>
                {
                    if (_raceStarted && !_resultSent)
                    {
                        await _lapUpdateService.SendLapUpdatesAsync();
                    }
                }, null, TimeSpan.Zero, TimeSpan.FromMinutes(10));
            });

            // Schedule final result
            _ = Task.Run(async () =>
            {
                var resultDelay = raceEnd - DateTime.Now;
                if (resultDelay > TimeSpan.Zero)
                    await Task.Delay(resultDelay);

                _resultSent = true;
                _raceStarted = false;

                if (_lapUpdateTimer != null)
                    _lapUpdateTimer.Dispose();

                await _raceResultService.SendFinalRaceStandingsAsync();
            });
        }
    }
}