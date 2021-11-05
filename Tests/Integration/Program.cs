using Microsoft.Extensions.Configuration;
using Spectre.Console;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using TheSwamp.Shared;


namespace Integration
{
    internal static class Program
    {
        private static IConfiguration _cfg;
        
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
                                "Get details"
                            }));

                switch (cmd)
                {
                    case "Get details":
                        AnsiConsole.WriteLine("getting device info");
                        var x = await API.GetDeviceAsync("test-01");
                        AnsiConsole.WriteLine(x.ToString());
                        break;
                }
            }
        }

        
    }
}
