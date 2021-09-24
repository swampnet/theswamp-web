using Microsoft.Extensions.Configuration;
using Spectre.Console;
using System;
using System.Threading.Tasks;
using TheSwamp.Shared;


namespace Integration
{
    internal class Program
    {
        private static IConfiguration _cfg;

        static async Task Main(string[] args)
        {
             _cfg = new ConfigurationBuilder()
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                //.AddCommandLine(args)
                .Build();

            AnsiConsole.WriteLine("Press key to get device info!");
            Console.ReadKey();

            AnsiConsole.WriteLine("getting device info");
            API.Initialise(_cfg["api:endpoint"], _cfg["api:key"]);

            var x = await API.GetDeviceAsync("test-01");
            AnsiConsole.WriteLine(x.ToString());
            Console.ReadKey();
        }
    }
}
