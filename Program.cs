using Discord;
using F1UpdatesBot.Src;
using F1UpdatesBot.Src.Services;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using F1UpdatesBot.Src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Discord.WebSocket;
using F1UpdatesBot.Src.Services.Interfaces;
using F1UpdatesBot.Src.Models;

namespace F1UpdatesBot
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
    {
        services.AddHttpClient<Openf1Service>(client =>
        {
            client.BaseAddress = new Uri("https://api.openf1.org/v1/");
        });

        services.AddSingleton(provider =>
        {
            var config = new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.Guilds | GatewayIntents.GuildMessages
            };
            return new DiscordSocketClient(config);
        });

        services.AddSingleton<SessionsService>();
        services.AddSingleton<LapUpdateService>();
        services.AddSingleton<RaceResultService>();
        services.AddSingleton<SchedulerService>();
        services.AddSingleton<DriverService>();
        services.AddSingleton<Bot>();
    })
    .Build();


            var bot = host.Services.GetRequiredService<Bot>();
            await bot.RunAsync();
        }

        private static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
