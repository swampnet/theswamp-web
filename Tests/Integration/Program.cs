using Microsoft.Extensions.Configuration;
using Spectre.Console;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using TheSwamp.Shared;


namespace Integration
{
    internal class Program
    {
        private static IConfiguration _cfg;
        private static TheSwamp.Shared.Monitor _data = new TheSwamp.Shared.Monitor();

        static async Task Main(string[] args)
        {
             _cfg = new ConfigurationBuilder()
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                //.AddCommandLine(args)
                .Build();

            API.Initialise(_cfg["api:endpoint"], _cfg["api:key"]);


            while (true)
            {
                var cmd = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("Select [green]option[/]?")
                            .MoreChoicesText("[grey](Move up and down)[/]")
                            .AddChoices(new[] { 
                                "Get details", 
                                "Flush", 
                                "Start" }));

                switch (cmd)
                {
                    case "Start":
                        var timer = new Timer(AddData, null, 0, 1000);
                        AnsiConsole.WriteLine("start");
                        break;

                    case "Get details":
                        AnsiConsole.WriteLine("getting device info");
                        var x = await API.GetDeviceAsync("test-01");
                        AnsiConsole.WriteLine(x.ToString());
                        break;

                    case "Flush":
                        AnsiConsole.WriteLine("Flushing");
                        await _data.FlushAsync();
                        AnsiConsole.WriteLine("flush complete");
                        break;

                }
            }
        }

        private static Random _rng = new Random();

        private static async void AddData(object state)
        {
            var n = _rng.Next(1, 10);
            Debug.WriteLine($"RNG rolled {n}");
            await _data.AddDataPointAsync("test-01", n);
            await _data.AddDataPointAsync("test-02", n);
        }
    }
}
