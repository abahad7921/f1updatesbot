using Discord.WebSocket;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Channels;
using Newtonsoft.Json.Linq;
using DotNetEnv;

namespace F1UpdatesBot.Src.Services
{
    public class DriverService
    {
        private readonly Openf1Service _openF1Service;
        private IMessageChannel? _channel;
        private readonly DiscordSocketClient _client;
        private readonly ulong _channelId;
        private readonly TaskCompletionSource _channelReadyTcs = new();

        public DriverService(Openf1Service openF1Service, DiscordSocketClient client)
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

        public async Task SednDriverLineUpAsync(string location)
        {
            await _channelReadyTcs.Task; // ✅ Wait until channel is set
            _channel ??= await _client.Rest.GetChannelAsync(_channelId) as IMessageChannel;
            var drivers = await _openF1Service.GetDriversAsync();
            string message = $"**Driver Lineup for {location} Grand Prix:**\n";
            foreach (var driver in drivers)
            {
                message += $"- {driver.FullName} ({driver.TeamName})\n";
            }

            await _channel.SendMessageAsync(message);
        }
    }
}
