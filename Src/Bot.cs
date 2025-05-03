using Discord.WebSocket;
using Discord;
using F1UpdatesBot.Src.Services;
using F1UpdatesBot.Src.Services.Interfaces;
using DotNetEnv;

namespace F1UpdatesBot.Src
{
    public class Bot
    {
        private readonly DiscordSocketClient _client;
        private readonly SchedulerService _schedulerService;
        private readonly string _token;
     
        public Bot(DiscordSocketClient client, SchedulerService schedulerService)
        {
            Env.Load();
            _client = client;
            _schedulerService = schedulerService;
            _token = Environment.GetEnvironmentVariable("BOT_TOKEN");
        }

        public async Task RunAsync()
        {
            _client.Log += LogAsync;

            var readyTcs = new TaskCompletionSource();

            _client.Ready += () =>
            {
                Console.WriteLine("Discord client is ready.");
                readyTcs.SetResult();
                return Task.CompletedTask;
            };

            await _client.LoginAsync(TokenType.Bot, _token);
            await _client.StartAsync();

            // Wait until Discord is fully ready
            await readyTcs.Task;
            await _schedulerService.StartAsync();
            await Task.Delay(-1); // Keep bot running
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }
    }
}
