using Discord;
using F1UpdatesBot.Src.Services;
using Newtonsoft.Json;

namespace F1UpdatesBot
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var sessionService = new SessionsService();
            //var res = await sessionService.getAll();
            //Console.WriteLine(JsonConvert.SerializeObject(res));

            var res1 = await sessionService.getCurrentSessionKey();
            Console.Write(JsonConvert.SerializeObject(res1));
        }

        private static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
