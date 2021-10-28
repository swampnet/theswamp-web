using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheSwamp.Shared;

namespace Agent
{
    internal static class Program
    {
        public static IConfiguration Cfg { get; private set; }

        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
#if DEBUG
            builder.AddJsonFile("local.settings.json", optional: false, reloadOnChange: true);
#else
            builder.AddJsonFile("settings.json", optional: false, reloadOnChange: true);
#endif
            Cfg = builder.Build();

            API.Initialise(Cfg["api:endpoint"], Cfg["api:key"]);
            
            if (!await ProcessMessages(args))
            {
                await MainLoopAsync();
            }

            Console.WriteLine("fin");
        }


        private static async Task<bool> ProcessMessages(string[] args)
        {
            var msg = args.Where(a => a.StartsWith("led-message:"));
            if (msg.Any())
            {
                foreach (var m in msg)
                {
                    var content = m.Substring(m.IndexOf(":") + 1).Replace("_", " ");
                    Console.WriteLine($"msg: {content}");
                    await API.PostMessageAsync(new AgentMessage()
                    {
                        Type = "led-matrix",
                        Properties = new List<Property>() {
                            new Property("content", content)
                        }
                    });
                }

                return true;
            }
            return false;
        }


        private static async Task MainLoopAsync()
        {
            Console.WriteLine("Running main loop");

            var lastRun = new Dictionary<IThing, DateTime>();
            var monitor = new Monitor(60000 * 5);
            //var monitor = new Monitor(10000);

            // @TODO: Inject this stuff
            var things = new List<IThing>()
            {
                new RandomNumberThing(),
                new SendMessageThing(),
                new SensorHubThing(),
                new Mcp3008Things(),
                new CpuTemperatureThing()
            };

            var queueHandler = new AgentQueueHandler(Cfg);
            await queueHandler.StartAsync();

            while (true)
            {
                foreach (var t in things)
                {
                    try
                    {
                        if (!lastRun.ContainsKey(t))
                        {
                            Console.WriteLine($"Add {t.GetType().Name}");
                            lastRun.Add(t, DateTime.MinValue);
                        }

                        var diff = (DateTime.UtcNow - lastRun[t]).TotalSeconds;
                        if (diff > t.PollInterval.TotalSeconds)
                        {
                            //Console.WriteLine($"[{DateTime.UtcNow}] Poll {t.GetType().Name} ({diff})");
                            await t.PollAsync(monitor);
                            lastRun[t] = DateTime.UtcNow;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }

                await Task.Delay(500);
            }
        }
    }
}
