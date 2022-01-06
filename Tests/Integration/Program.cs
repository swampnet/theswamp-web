using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Spectre.Console;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TheSwamp.Shared;


namespace Integration
{
    internal static class Program
    {
        private static IConfiguration _cfg;
        
        private static Pump[] _pumps = new[] { 
            new Pump("Pump 1 (38) YELLOW", 38),
            new Pump("Pump 2 (40) BLUE", 40),
            new Pump("Pump 3 (36) RED", 36),
            new Pump("Pump 4 (32) GREEN", 32)
        };

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
                        .Title("Select [green]option[/] ?")
                        .MoreChoicesText("[grey](Move up and down)[/]")
                        .AddChoices(new[] { 
                            "Get details"
                        })
                        .AddChoices(_pumps.Select(t => t.Description))
                );

                switch (cmd)
                {
                    case "Get details":
                        AnsiConsole.WriteLine("getting device info");
                        var x = await API.GetDeviceAsync("test-01");
                        AnsiConsole.WriteLine(x.ToString());
                        break;

                    default:
                        var t = _pumps.SingleOrDefault(x => x.Description == cmd);
                        if(t != null)
                        {
                            AnsiConsole.WriteLine($"Activating pump {t.Channel}");

                            await EnqueueAsync(new AgentMessage()
                            {
                                Type = "activate-pump",
                                Properties = new System.Collections.Generic.List<Property>() {
                                    new Property("channel", t.Channel.ToString())
                                }
                            });
                        }
                        break;
                }
            }
        }


        private static async Task EnqueueAsync(AgentMessage msg)
        {
            await using (ServiceBusClient client = new ServiceBusClient(_cfg["azure.servicebus"]))
            {
                var sender = client.CreateSender("iot_agent");
                var message = new ServiceBusMessage(JsonConvert.SerializeObject(msg));
                await sender.SendMessageAsync(message);
            }
        }


        class Pump
        {
            public Pump(string description, int channel)
            {
                Description = description;
                Channel = channel;
            }
            public string Description { get; private set; }
            public int Channel { get; private set; }
        }
    }
}
