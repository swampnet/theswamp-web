using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
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
                                "Get details",
                                "Pump 1",
                                "Pump 2"
                            }));

                switch (cmd)
                {
                    case "Get details":
                        AnsiConsole.WriteLine("getting device info");
                        var x = await API.GetDeviceAsync("test-01");
                        AnsiConsole.WriteLine(x.ToString());
                        break;

                    case "Pump 1":
                        await EnqueueAsync(new AgentMessage() {
                            Type = "activate-pump",
                            Properties = new System.Collections.Generic.List<Property>() { 
                                new Property("channel", "38")
                            }
                        });
                        break;

                    case "Pump 2":
                        await EnqueueAsync(new AgentMessage()
                        {
                            Type = "activate-pump",
                            Properties = new System.Collections.Generic.List<Property>() {
                                new Property("channel", "40")
                            }
                        });
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
    }
}
